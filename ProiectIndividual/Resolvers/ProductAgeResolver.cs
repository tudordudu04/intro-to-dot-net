using AutoMapper;
using ProiectIndividual.Products;

namespace ProiectIndividual.Validators;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDTO, string>
{
    public string Resolve(Product source, ProductProfileDTO destination, string destMember, ResolutionContext context)
    {
        var now = DateTime.UtcNow.Date;
        var release = source.ReleaseDate.Date;
        if (release > now) return "Releases in the future";

        var totalDays = (now - release).Days;

        // Exact 5 years (1825 days) => "Classic"
        if (totalDays == 1825) return "Classic";

        if (totalDays < 30) return "New Release";

        if (totalDays < 365)
        {
            var months = Math.Max(1, (now.Year - release.Year) * 12 + now.Month - release.Month);
            return $"{months} month{(months == 1 ? "" : "s")} old";
        }

        // < 1825 days (handled above for equality) => X years old
        var years = (int)(totalDays / 365.0);
        if (totalDays < 1825)
        {
            years = Math.Max(1, years);
            return $"{years} year{(years == 1 ? "" : "s")} old";
        }

        // > 1825 days (more than 5 years): also show years
        years = Math.Max(1, years);
        return $"{years} year{(years == 1 ? "" : "s")} old";
    }
}