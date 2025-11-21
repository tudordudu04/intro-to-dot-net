using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Persistance;
using ProiectIndividual.Products;
using ProiectIndividual.Requests;
using ProiectIndividual.Logging;
using ProiectIndividual.Exceptions;
using FluentValidation;

namespace ProiectIndividual.Handlers;

public class CreateProductHandler(
    ProductManagementContext context,
    IMapper mapper,
    ILogger<CreateProductHandler> logger,
    IHttpContextAccessor httpContextAccessor,
    IValidator<CreateProductProfileRequest> validator)
{
    public async Task<IResult> Handle(CreateProductProfileRequest request)
    {
        var operationId = GenerateOperationId();
        var correlationId = httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                            ?? httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty;

        using var op = new ProductCreationOperation(logger, operationId, correlationId, request.Name, request.SKU, request.Category, request.Brand);

        var validationResult = await op.TimeAsync<FluentValidation.Results.ValidationResult>("validation", () => validator.ValidateAsync(request));
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            logger.LogWarning(LogEvents.ProductValidationFailed,
                "Validation failed for OperationId:{OperationId} Name:{Name} SKU:{SKU} Errors:{Errors}",
                operationId, request.Name, request.SKU, string.Join("; ", errors));

            op.CompleteFailure(string.Join("; ", errors));
            throw new Exceptions.ValidationException(errors);
        }

        var skuExists = await op.TimeAsync<bool>("sku", () => context.Products.AnyAsync(p => p.SKU == request.SKU));
        op.LogSkuValidation(skuExists);

        if (skuExists)
        {
            var reason = "SKU must be unique.";
            logger.LogWarning(LogEvents.ProductValidationFailed,
                "SKU uniqueness failed OperationId:{OperationId} Name:{Name} SKU:{SKU}",
                operationId, request.Name, request.SKU);

            op.CompleteFailure(reason);
            throw new Exceptions.ValidationException(reason);
        }

        op.LogStockValidation(request.StockQuantity);

        try
        {
            var product = mapper.Map<Product>(request);

            op.LogDatabaseStarted(product.Id);

            await op.TimeAsync<bool>("db", async () =>
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return true;
            });

            op.LogDatabaseCompleted(product.Id);

            op.LogCacheOperation();

            op.CompleteSuccess();

            var dto = mapper.Map<ProductProfileDTO>(product);
            logger.LogInformation("Product created successfully with ID: {Id} OperationId:{OperationId}", product.Id, operationId);

            return Results.Created($"/products/{dto.Id}", dto);
        }
        catch (Exception ex)
        {
            op.CompleteFailure(ex.Message);
            logger.LogError(ex, "Error creating product OperationId:{OperationId} Name:{Name} SKU:{SKU}", operationId, request.Name, request.SKU);
            throw;
        }
    }

    private static string GenerateOperationId()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
