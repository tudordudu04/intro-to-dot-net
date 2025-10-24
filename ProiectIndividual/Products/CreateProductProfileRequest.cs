namespace ProiectIndividual.Products;

public record CreateProductProfileRequest(string Name, string Brand, string SKU,
    ProductCategory Category, decimal Price, DateTime ReleaseDate, string? ImageUrl, int StockQuantity = 1);