namespace ProiectIndividual.Products;

public record ProductProfileDTO(Guid Id, string Name, string Brand, string SKU, string
    CategoryDisplayName, decimal Price, string FormattedPrice, DateTime ReleaseDate, DateTime CreatedAt,
    string? ImageUrl, bool IsAvailable, int StockQuantity, string ProductAge, string BrandInitials, string AvailabilityStatus);