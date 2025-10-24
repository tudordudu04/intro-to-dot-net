using ProiectIndividual.Persistance;

namespace ProiectIndividual.Products;

public class DeleteProductHandler
{
    private readonly ProductManagementContext context;

    public DeleteProductHandler(ProductManagementContext context)
    {
        this.context = context;
    }
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