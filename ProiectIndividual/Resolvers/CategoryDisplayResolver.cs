using AutoMapper;
using ProiectIndividual.Enums;
using ProiectIndividual.Products;

namespace ProiectIndividual.Resolvers;

public class CategoryDisplayResolver : IValueResolver<Product, ProductProfileDTO, string>
{
    public string Resolve(Product source, ProductProfileDTO destination, string destMember, ResolutionContext context)
    {
        return source.Category switch
        {
            ProductCategory.Electronics => "Electronics & Technology",
            ProductCategory.Clothing => "Clothing & Fashion",
            ProductCategory.Books => "Books & Media",
            ProductCategory.Home => "Home & Garden",
            _ => "Uncategorized"
        };
    }
}
