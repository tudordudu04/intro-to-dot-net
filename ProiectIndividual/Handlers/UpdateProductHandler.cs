using FluentValidation;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;
using ProiectIndividual.Validators;

namespace ProiectIndividual.Handlers;

public class UpdateProductHandler(ProductManagementContext context, ILogger<UpdateProductHandler> logger, IValidator<UpdateProductRequest> validator)
{
    public async Task<IResult> Handle(UpdateProductRequest request)
    {
        var product = await context.Products.FindAsync(request.Id);
        if (product == null)
        {
            return Results.NotFound($"Product with ID {request.Id} not found.");
        }

        // var validatorResults = await validator.ValidateAsync(request);
        // if(!validatorResults.IsValid)
        //     throw new ValidationException(validatorResults.Errors);
        //TODO mapper
        
        
        
        await context.SaveChangesAsync();

        return Results.Ok(product);
    }
}