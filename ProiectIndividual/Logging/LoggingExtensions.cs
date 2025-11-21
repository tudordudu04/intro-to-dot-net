namespace ProiectIndividual.Logging;

public static class LoggingExtensions
{
    public static void LogProductCreationMetrics(this ILogger logger, ProductCreationMetrics metrics)
    {
        if (metrics.Success)
        {
            logger.LogInformation(LogEvents.ProductCreationCompleted,
                "ProductCreationMetrics OperationId:{OperationId} Name:{Name} SKU:{SKU} Category:{Category} " +
                "ValidationMs:{ValidationMs} DatabaseMs:{DatabaseMs} TotalMs:{TotalMs} Success:{Success}",
                metrics.OperationId,
                metrics.ProductName,
                metrics.SKU,
                metrics.Category,
                metrics.ValidationDuration.TotalMilliseconds,
                metrics.DatabaseSaveDuration.TotalMilliseconds,
                metrics.TotalDuration.TotalMilliseconds,
                metrics.Success);
        }
        else
        {
            logger.LogInformation(LogEvents.ProductCreationCompleted,
                "ProductCreationMetrics OperationId:{OperationId} Name:{Name} SKU:{SKU} Category:{Category} " +
                "ValidationMs:{ValidationMs} DatabaseMs:{DatabaseMs} TotalMs:{TotalMs} Success:{Success} ErrorReason:{ErrorReason}",
                metrics.OperationId,
                metrics.ProductName,
                metrics.SKU,
                metrics.Category,
                metrics.ValidationDuration.TotalMilliseconds,
                metrics.DatabaseSaveDuration.TotalMilliseconds,
                metrics.TotalDuration.TotalMilliseconds,
                metrics.Success,
                metrics.ErrorReason ?? string.Empty);
        }
    }
}