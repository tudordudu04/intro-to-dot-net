using ProiectIndividual.Enums;

namespace ProiectIndividual.Products;

public record Product(
    Guid Id,
    string Name,
    string Brand,
    string SKU,
    ProductCategory Category,
    decimal Price,
    DateTime ReleaseDate,
    string? ImageUrl,
    bool IsAvailable,
    int StockQuantity = 0,
    DateTime CreatedAt = default,
    DateTime? UpdatedAt = null
);