using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;

namespace ProiectIndividual.Handlers;

public class GetProductByIdHandler(ProductManagementContext context)
{
    public async Task<IResult> Handle(GetProductByIdRequest request)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id);

        if (product == null)
            return Results.NotFound();
        
        return Results.Ok(product);
    }
}