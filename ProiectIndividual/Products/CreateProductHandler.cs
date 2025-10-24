using ProiectIndividual.Products;
using ProiectIndividual.Persistance;
using ProiectIndividual.Validators;

public class CreateProductHandler
{
    private readonly ILogger<CreateProductHandler> logger;
    private readonly ProductManagementContext context;
    public CreateProductHandler(ProductManagementContext context, ILogger<CreateProductHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }
    public async Task<IResult> Handle(CreateProductProfileRequest request)
    {
        logger.LogInformation("Creating new product with the name: {Name} and the price: {Price} ", request.Name, request.Price);
        // TODO - create a middleware for validation
        var validator = new CreateProductValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }
        var product = new Product(Guid.NewGuid(), request.Name, request.Brand, request.SKU,
            request.Category, request.Price, request.ReleaseDate, request.ImageUrl, request.StockQuantity);
        
        context.Products.Add(product);
        await context.SaveChangesAsync();
        logger.LogInformation("User created successfully with ID: {Id}", product.Id);

        return Results.Created($"/products/{product.Id}", product);
    }
}