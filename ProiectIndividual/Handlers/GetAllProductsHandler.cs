using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;

namespace ProiectIndividual.Handlers;

public class GetAllProductsHandler(ProductManagementContext context)
{
    public async Task<IResult> Handle(GetAllProductsRequest request)
    {
        var products = await context.Products.ToListAsync();
        return Results.Ok(products);
    }
}