using System.Diagnostics;
using ProiectIndividual.Enums;

namespace ProiectIndividual.Logging;
public sealed class ProductCreationOperation : IDisposable
{
    private static readonly NullScope _nullScope = NullScope.Instance;

    private readonly ILogger? _logger;
    private readonly IDisposable _scope;
    private readonly Stopwatch _totalSw = Stopwatch.StartNew();
    private readonly Dictionary<string, TimeSpan> _phases = new();
    private readonly Dictionary<string, Stopwatch> _running = new();
    private bool _disposed;

    public string OperationId { get; }
    public string CorrelationId { get; }
    public string ProductName { get; }
    public string SKU { get; }
    public ProductCategory Category { get; }

    public ProductCreationOperation(
        ILogger logger,
        string operationId,
        string? correlationId,
        string productName,
        string sku,
        ProductCategory category,
        string brand)
    {
        _logger = logger;
        OperationId = operationId;
        CorrelationId = correlationId ?? string.Empty;
        ProductName = productName;
        SKU = sku;
        Category = category;

        // Safely create scope (mocked logger may return null)
        _scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["OperationId"] = OperationId,
            ["CorrelationId"] = CorrelationId
        }) ?? _nullScope;

        _logger?.LogInformation(LogEvents.ProductCreationStarted,
            "Creating product Name:{Name} Brand:{Brand} Category:{Category} SKU:{SKU} OperationId:{OperationId}",
            ProductName, brand, Category, SKU, OperationId);
    }

    public void StartPhase(string name)
    {
        if (_disposed) return;
        _running[name] = Stopwatch.StartNew();
    }

    public void EndPhase(string name)
    {
        if (_disposed) return;
        if (_running.TryGetValue(name, out var sw))
        {
            sw.Stop();
            _phases[name] = sw.Elapsed;
            _running.Remove(name);
        }
    }

    public TimeSpan GetPhase(string name) => _phases.TryGetValue(name, out var t) ? t : TimeSpan.Zero;

    public async Task<T> TimeAsync<T>(string phaseName, Func<Task<T>> action)
    {
        StartPhase(phaseName);
        try
        {
            var result = await action();
            EndPhase(phaseName);
            return result;
        }
        catch
        {
            EndPhase(phaseName);
            throw;
        }
    }

    public void LogSkuValidation(bool exists)
    {
        var skuMs = GetPhase("sku").TotalMilliseconds;
        _logger?.LogInformation(LogEvents.SKUValidationPerformed,
            "SKU validation performed OperationId:{OperationId} SKU:{SKU} Exists:{Exists} DurationMs:{Ms}",
            OperationId, SKU, exists, skuMs);
    }

    public void LogStockValidation(int stock)
    {
        _logger?.LogInformation(LogEvents.StockValidationPerformed,
            "Stock validation performed OperationId:{OperationId} SKU:{SKU} Stock:{Stock}",
            OperationId, SKU, stock);
    }

    public void LogDatabaseStarted(Guid productId)
    {
        _logger?.LogInformation(LogEvents.DatabaseOperationStarted,
            "Database operation started OperationId:{OperationId} SKU:{SKU} ProductId:{ProductId}",
            OperationId, SKU, productId);
    }

    public void LogDatabaseCompleted(Guid productId)
    {
        var dbMs = GetPhase("db").TotalMilliseconds;
        _logger?.LogInformation(LogEvents.DatabaseOperationCompleted,
            "Database operation completed OperationId:{OperationId} ProductId:{ProductId} DurationMs:{Ms}",
            OperationId, productId, dbMs);
    }

    public void LogCacheOperation(string cacheKey = "all_products")
    {
        _logger?.LogInformation(LogEvents.CacheOperationPerformed,
            "Cache operation performed OperationId:{OperationId} CacheKey:{CacheKey}",
            OperationId, cacheKey);
    }

    public void CompleteSuccess()
    {
        if (_disposed) return;
        _totalSw.Stop();
        var metrics = new ProductCreationMetrics(
            OperationId,
            ProductName,
            SKU,
            Category,
            GetPhase("validation") + GetPhase("sku"),
            GetPhase("db"),
            _totalSw.Elapsed,
            true
        );
        _logger?.LogProductCreationMetrics(metrics);
    }

    public void CompleteFailure(string? errorReason)
    {
        if (_disposed) return;
        _totalSw.Stop();
        var metrics = new ProductCreationMetrics(
            OperationId,
            ProductName,
            SKU,
            Category,
            GetPhase("validation") + GetPhase("sku"),
            GetPhase("db"),
            _totalSw.Elapsed,
            false,
            errorReason
        );
        _logger?.LogProductCreationMetrics(metrics);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            foreach (var kv in _running)
            {
                kv.Value.Stop();
                _phases[kv.Key] = kv.Value.Elapsed;
            }
            _running.Clear();
        }
        catch
        {
            // swallow timing cleanup errors
        }

        try
        {
            _scope.Dispose(); // safe due to NullScope
        }
        catch
        {
            // swallow scope dispose errors
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        private NullScope() { }
        public void Dispose() { }
    }
}
