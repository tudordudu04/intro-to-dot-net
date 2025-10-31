using System.Globalization;
using AutoMapper;
using ProiectIndividual.Enums;
using ProiectIndividual.Products;

namespace ProiectIndividual.Resolvers;

public class PriceFormatterResolver : IValueResolver<Product, ProductProfileDTO, string>
{
    private readonly CultureInfo _culture;

    public PriceFormatterResolver()
    {
        _culture = CultureInfo.CurrentCulture;
    }

    public string Resolve(Product source, ProductProfileDTO destination, string destMember, ResolutionContext context)
    {
        decimal effectivePrice = source.Category == ProductCategory.Home
            ? source.Price * 0.9m
            : source.Price;

        // Format with two decimal places and culture currency symbol.
        return effectivePrice.ToString("C2", _culture);
    }
}
