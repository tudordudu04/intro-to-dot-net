using AutoMapper;
using ProiectIndividual.Products;

namespace ProiectIndividual.Resolvers;

public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDTO, string>
{
    public string Resolve(Product source, ProductProfileDTO destination, string destMember, ResolutionContext context)
    {
        var parts = source.Brand
            .Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToArray();

        if (parts.Length == 0) return "?";

        if (parts.Length == 1)
        {
            return char.ToUpperInvariant(parts[0][0]).ToString();
        }

        var first = char.ToUpperInvariant(parts.First()[0]);
        var last = char.ToUpperInvariant(parts.Last()[0]);
        return $"{first}{last}";
    }
}