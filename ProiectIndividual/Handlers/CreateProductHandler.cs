using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Persistance;
using ProiectIndividual.Products;
using ProiectIndividual.Requests;
using ProiectIndividual.Validators;

namespace ProiectIndividual.Handlers;

public class CreateProductHandler(ProductManagementContext context, IMapper mapper, ILogger<CreateProductHandler> logger)
{
    public async Task<IResult> Handle(CreateProductProfileRequest request)
    {
        logger.LogInformation("Creating product Name:{Name} Brand:{Brand} Category:{Category} SKU:{SKU} Price:{Price}",
            request.Name, request.Brand, request.Category, request.SKU, request.Price);

        var validator = new CreateProductValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        if (await context.Products.AnyAsync(p => p.SKU == request.SKU))
            return Results.Conflict("SKU must be unique.");

        var product = mapper.Map<Product>(request);

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var dto = mapper.Map<ProductProfileDTO>(product);

        logger.LogInformation("Product created successfully with ID: {Id}", product.Id);
        return Results.Created($"/products/{dto.Id}", dto);
    }
}