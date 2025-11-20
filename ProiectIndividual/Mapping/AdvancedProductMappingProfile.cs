using AutoMapper;
using ProiectIndividual.Products;
using ProiectIndividual.Requests;
using ProiectIndividual.Resolvers;
using ProiectIndividual.Validators;
using ProiectIndividual.Enums;

namespace ProiectIndividual.Mapping;

public class AdvancedProductMappingProfile : Profile
{
    private static readonly CategoryDisplayResolver CategoryResolver = new();
    private static readonly PriceFormatterResolver PriceResolver = new();
    private static readonly ProductAgeResolver AgeResolver = new();
    private static readonly BrandInitialsResolver InitialsResolver = new();
    private static readonly AvailabilityStatusResolver StatusResolver = new();
    public AdvancedProductMappingProfile()
    {
        CreateMap<CreateProductProfileRequest, Product>()
            .ConstructUsing(src => new Product(
                Guid.NewGuid(),                    
                src.Name,
                src.Brand,
                src.SKU,
                src.Category,
                src.Price,
                src.ReleaseDate,
                src.ImageUrl,
                src.StockQuantity > 0,
                src.StockQuantity,
                DateTime.UtcNow,
                null
            ));
        CreateMap<Product, ProductProfileDTO>()
            .ConstructUsing((src, ctx) => new ProductProfileDTO(
                src.Id,
                src.Name,
                src.Brand,
                src.SKU,
                CategoryResolver.Resolve(src, null, null, ctx),
                0,
                PriceResolver.Resolve(src, null, null, ctx),
                src.ReleaseDate,
                src.CreatedAt == default ? DateTime.UtcNow : src.CreatedAt,
                src.ImageUrl,
                src.IsAvailable,
                src.StockQuantity,
                AgeResolver.Resolve(src, null, null, ctx),
                InitialsResolver.Resolve(src, null, null, ctx),
                StatusResolver.Resolve(src, null, null, ctx)
            ))
            .ForMember(d => d.Price,
                opt => opt.MapFrom(src =>
                    src.Category == ProductCategory.Home ? src.Price * 0.9m : src.Price))
            .ForMember(d => d.ImageUrl, opt => opt.MapFrom(src =>
                src.Category == ProductCategory.Home ? null : src.ImageUrl));
    }
}