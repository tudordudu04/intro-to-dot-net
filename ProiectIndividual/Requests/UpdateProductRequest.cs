using ProiectIndividual.Enums;

namespace ProiectIndividual.Requests;

public record UpdateProductRequest(
    Guid Id,
    string? Name,
    string? Brand,
    string? SKU,
    ProductCategory? Category,
    decimal? Price,
    DateTime? ReleaseDate,
    string? ImageUrl,
    bool? IsAvailable,
    int? StockQuantity,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
);