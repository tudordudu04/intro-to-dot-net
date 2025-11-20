using AutoMapper;
using ProiectIndividual.Products;

namespace ProiectIndividual.Validators;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDTO, string>
{
    public string Resolve(Product source, ProductProfileDTO destination, string destMember, ResolutionContext context)
    {
        var now = DateTime.UtcNow.Date;
        var release = source.ReleaseDate.Date;
        if (release > now) return "future";

        var totalDays = (now - release).Days;
        if (totalDays == 1825) return "classic";
        if (totalDays < 30) return "new";

        if (totalDays < 365)
        {
            var months = Math.Max(1, (now.Year - release.Year) * 12 + now.Month - release.Month);
            return $"{months}m";
        }

        var years = Math.Max(1, (int)(totalDays / 365.0));
        return $"{years}y";
    }
}