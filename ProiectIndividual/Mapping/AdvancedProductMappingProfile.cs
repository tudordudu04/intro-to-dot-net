using AutoMapper;
using ProiectIndividual.Products;
using ProiectIndividual.Requests;
using ProiectIndividual.Resolvers;
using ProiectIndividual.Validators;
using ProiectIndividual.Enums;

namespace ProiectIndividual.Mapping;

public class AdvancedProductMappingProfile : Profile
{
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
            .ForMember(dest => dest.CategoryDisplayName,
                opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(dest => dest.ProductAge,
                opt => opt.MapFrom<ProductAgeResolver>())
            .ForMember(dest => dest.BrandInitials,
                opt => opt.MapFrom<BrandInitialsResolver>())
            .ForMember(dest => dest.AvailabilityStatus,
                opt => opt.MapFrom<AvailabilityStatusResolver>())
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src =>
                src.Category == ProductCategory.Home ? src.Price * 0.9m : src.Price))
            .ForMember(dest => dest.FormattedPrice,
                opt => opt.MapFrom<PriceFormatterResolver>())
            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src =>
                src.Category == ProductCategory.Home ? null : src.ImageUrl))
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Brand,
                opt => opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.SKU,
                opt => opt.MapFrom(src => src.SKU))
            .ForMember(dest => dest.ReleaseDate,
                opt => opt.MapFrom(src => src.ReleaseDate))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt == default ? DateTime.UtcNow : src.CreatedAt))
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.IsAvailable))
            .ForMember(dest => dest.StockQuantity,
                opt => opt.MapFrom(src => src.StockQuantity));
    }
}