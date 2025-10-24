using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Persistance;

namespace ProiectIndividual.Products;

public class GetAllProductsHandler
{
    private readonly ProductManagementContext context;

    public GetAllProductsHandler(ProductManagementContext context)
    {
        this.context = context;
    }

    public async Task<IResult> Handle(GetAllProductsRequest request)
    {
        var products = await context.Products.ToListAsync();
        return Results.Ok(products);
    }
}