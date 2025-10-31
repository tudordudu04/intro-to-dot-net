using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;

namespace ProiectIndividual.Handlers;

public class DeleteProductHandler(ProductManagementContext context)
{
    public async Task<IResult> Handle(DeleteProductRequest request)
    {
        var product = await context.Products.FindAsync(request.Id);
        if (product == null)
        {
            return Results.NotFound();
        }
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}