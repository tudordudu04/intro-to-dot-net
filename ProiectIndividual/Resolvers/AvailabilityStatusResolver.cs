using AutoMapper;
using ProiectIndividual.Products;

namespace ProiectIndividual.Resolvers;

public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDTO, string>
{
    public string Resolve(Product source, ProductProfileDTO destination, string destMember, ResolutionContext context)
    {
        if (!source.IsAvailable)
            return "Out of Stock";

        var qty = source.StockQuantity;

        if (qty == 0)
            return "Unavailable";

        if (qty == 1)
            return "Last Item";

        if (qty <= 5)
            return "Limited Stock";

        return "In Stock";
    }
}