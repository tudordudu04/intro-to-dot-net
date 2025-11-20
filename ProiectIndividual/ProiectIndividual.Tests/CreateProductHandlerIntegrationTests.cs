using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ProiectIndividual.Enums;
using ProiectIndividual.Handlers;
using ProiectIndividual.Mapping;
using ProiectIndividual.Persistance;
using ProiectIndividual.Products;
using ProiectIndividual.Requests;
using ProiectIndividual.Validators;
using Xunit;

namespace ProiectIndividual.ProiectIndividual.Tests;

public class CreateProductHandlerIntegrationTests : IDisposable
{
    private readonly ProductManagementContext _context;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IValidator<CreateProductProfileRequest> _validator;
    private readonly MemoryCache _cache;
    private readonly CreateProductHandler _handler;
    private readonly ServiceProvider _services;

    public CreateProductHandlerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ProductManagementContext>()
            .UseInMemoryDatabase($"Products_{Guid.NewGuid():N}")
            .Options;
        _context = new ProductManagementContext(options);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<AdvancedProductMappingProfile>(),
            typeof(AdvancedProductMappingProfile));
        _services = services.BuildServiceProvider();
        _mapper = _services.GetRequiredService<IMapper>();

        _loggerMock = new Mock<ILogger<CreateProductHandler>>();

        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = Guid.NewGuid().ToString();
        httpContext.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
        _httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        var validatorLogger = LoggerFactory.Create(b => { }).CreateLogger<CreateProductValidator>();
        _validator = new CreateProductValidator(_context, validatorLogger);


        _handler = new CreateProductHandler(_context, _mapper, _loggerMock.Object, _httpContextAccessor, _validator);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
    {
        var req = new CreateProductProfileRequest(
            "Wireless Battery Processor",
            "Mega Tech",
            "ELEC-12345",
            ProductCategory.Electronics,
            199.99m,
            DateTime.UtcNow.AddMonths(-6),
            "https://example.com/image.jpg",
            15
        );

        var result = await _handler.Handle(req);

        Assert.IsType<Created<ProductProfileDTO>>(result);
        var valueResult = result as IValueHttpResult;
        Assert.NotNull(valueResult);
        var dto = valueResult!.Value as ProductProfileDTO;
        Assert.NotNull(dto);

        Assert.Equal("Electronics & Technology", dto.CategoryDisplayName);
        Assert.Equal("MT", dto.BrandInitials);
        var numericPart = new string(dto.ProductAge.TakeWhile(char.IsDigit).ToArray());
        Assert.True(int.TryParse(numericPart, out var ageValue) && ageValue > 0);
        Assert.StartsWith(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, dto.FormattedPrice);
        Assert.Equal("In Stock", dto.AvailabilityStatus);

        _loggerMock.Verify(l =>
            l.Log(
                It.Is<LogLevel>(ll => ll == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Product created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateSKU_ThrowsValidationExceptionWithLogging()
    {
        var seedReq = new CreateProductProfileRequest(
            "Existing Product",
            "Test Brand",
            "DUP-00001",
            ProductCategory.Home,
            50m,
            DateTime.UtcNow.AddMonths(-2),
            null,
            5
        );
        var existing = _mapper.Map<Product>(seedReq);
        _context.Products.Add(existing);
        await _context.SaveChangesAsync();

        var req = new CreateProductProfileRequest(
            "Another Product",
            "Test Brand",
            "DUP-00001",
            ProductCategory.Home,
            75m,
            DateTime.UtcNow.AddMonths(-1),
            null,
            3
        );

        var ex = await Assert.ThrowsAsync<global::ProiectIndividual.Exceptions.ValidationException>(() => _handler.Handle(req));
        Assert.Contains("unique", string.Join("; ", ex.Errors), StringComparison.OrdinalIgnoreCase);

        _loggerMock.Verify(l =>
            l.Log(
                It.Is<LogLevel>(ll => ll == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("SKU uniqueness failed") || o.ToString()!.Contains("Validation failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_HomeProductRequest_AppliesDiscountAndConditionalMapping()
    {
        var req = new CreateProductProfileRequest(
            "Soft Cushion",
            "Home Goods",
            "HOME-99999",
            ProductCategory.Home,
            100m,
            DateTime.UtcNow.AddMonths(-3),
            "https://example.com/cushion.jpg",
            50
        );

        var result = await _handler.Handle(req);

        Assert.IsType<Created<ProductProfileDTO>>(result);
        var valueResult = result as IValueHttpResult;
        var dto = valueResult?.Value as ProductProfileDTO;
        Assert.NotNull(dto);

        Assert.Equal("Home & Garden", dto!.CategoryDisplayName);
        Assert.Equal(90m, dto.Price);
        Assert.Null(dto.ImageUrl);

        _loggerMock.Verify(l =>
            l.Log(
                It.Is<LogLevel>(ll => ll == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Product created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
