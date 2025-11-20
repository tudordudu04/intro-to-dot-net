using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;
using ProiectIndividual.Enums;

namespace ProiectIndividual.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductProfileRequest>
{
    private readonly ProductManagementContext _db;
    private readonly ILogger<CreateProductValidator> _logger;

    private static readonly string[] InappropriateWords = new[] { "badword1", "badword2", "inappropriate" };
    private static readonly string[] HomeRestrictedWords = new[] { "nsfw-home", "restricted-home" };
    private static readonly string[] TechKeywords = new[] { "battery", "processor", "wireless", "bluetooth", "usb", "cpu", "gpu", "ssd" };
    private static readonly object[] AllCategories = Enum.GetValues(typeof(ProductCategory)).Cast<object>().ToArray();

    public CreateProductValidator(ProductManagementContext db, ILogger<CreateProductValidator> logger)
    {
        _db = db;
        _logger = logger;
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MinimumLength(1).WithMessage("Product name must be at least 1 character.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.")
            .Must(BeValidName).WithMessage("Product name contains inappropriate content.")
            .MustAsync(BeUniqueName).WithMessage("A product with the same name already exists for this brand.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MinimumLength(2).WithMessage("Brand must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Brand must not exceed 100 characters.")
            .Must(BeValidBrandName).WithMessage("Brand contains invalid characters.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .Must(s => new ValidSKUAttribute().IsValid(s))
            .WithMessage("SKU must be alphanumeric with optional hyphens and 5-20 characters.")
            .MustAsync(BeUniqueSKU).WithMessage("SKU must be unique.");

        RuleFor(x => x.Category)
            .Must(cat => new ProductCategoryAttribute(AllCategories).IsValid(cat))
            .WithMessage("Category must be a valid value.");

        RuleFor(x => x.Price)
            .Must(price => new PriceRangeAttribute(0.01, 9999.99).IsValid(price))
            .WithMessage("Price must be between $0.01 and $9,999.99.");

        RuleFor(x => x.ReleaseDate)
            .Must(date => date <= DateTime.UtcNow).WithMessage("Release date cannot be in the future.")
            .Must(date => date.Year >= 1900).WithMessage("Release date cannot be before year 1900.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.")
            .LessThanOrEqualTo(100000).WithMessage("Stock quantity cannot exceed 100,000.");

        When(x => !string.IsNullOrWhiteSpace(x.ImageUrl), () =>
        {
            RuleFor(x => x.ImageUrl!)
                .Must(BeValidImageUrl).WithMessage("ImageUrl must be an absolute http(s) URL pointing to an image file (.jpg, .jpeg, .png, .gif, .webp).");
        });

        When(x => x.Category == ProductCategory.Electronics, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(50m).WithMessage("Electronics must have a minimum price of $50.00.");

            RuleFor(x => x.Name)
                .Must(ContainTechnologyKeywords).WithMessage("Electronics product name must include technology-related keywords.");

            RuleFor(x => x.ReleaseDate)
                .Must(date => date >= DateTime.UtcNow.AddYears(-5)).WithMessage("Electronics must be released within the last 5 years.");
        });

        When(x => x.Category == ProductCategory.Home, () =>
        {
            RuleFor(x => x.Price)
                .LessThanOrEqualTo(200m).WithMessage("Home products must have a maximum price of $200.00.");

            RuleFor(x => x.Name)
                .Must(BeAppropriateForHome).WithMessage("Home product name contains restricted content.");
        });

        When(x => x.Category == ProductCategory.Clothing, () =>
        {
            RuleFor(x => x.Brand)
                .MinimumLength(3).WithMessage("Clothing brand name must be at least 3 characters.");
        });

        RuleFor(x => x)
            .Must(x => !(x.Price > 500m && x.StockQuantity > 10))
            .WithMessage("Expensive products (price > $100) must have stock <= 20 units.")
            .WithSeverity(Severity.Error);

        RuleFor(x => x)
            .MustAsync(PassBusinessRules)
            .WithMessage("Business rules validation failed.");
    }

    private bool BeValidName(string name)
    {
        var lowered = name.ToLowerInvariant();
        foreach (var bad in InappropriateWords)
        {
            if (lowered.Contains(bad.ToLowerInvariant()))
            {
                _logger.LogInformation("Name failed inappropriate content check: {Name} (matched {Word})", name, bad);
                return false;
            }
        }
        return true;
    }

    private async Task<bool> BeUniqueName(CreateProductProfileRequest req, string name, CancellationToken ct)
    {
        var exists = await _db.Products.AnyAsync(p => p.Name == name && p.Brand == req.Brand, ct);
        if (exists) _logger.LogInformation("Name+Brand uniqueness check failed for Name:{Name} Brand:{Brand}", name, req.Brand);
        return !exists;
    }

    private bool BeValidBrandName(string brand)
    {
        var regex = new Regex(@"^[\p{L}0-9\s\-\.'’]+$");
        var ok = regex.IsMatch(brand);
        if (!ok) _logger.LogInformation("Brand validation failed for {Brand}", brand);
        return ok;
    }

    private async Task<bool> BeUniqueSKU(string sku, CancellationToken ct)
    {
        var cleaned = sku?.Replace(" ", "") ?? string.Empty;
        var exists = await _db.Products.AnyAsync(p => p.SKU == cleaned, ct);
        if (exists) _logger.LogInformation("SKU uniqueness check failed for {SKU}", sku);
        return !exists;
    }

    private bool BeValidImageUrl(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        if (!(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) return false;
        var lower = uri.AbsolutePath.ToLowerInvariant();
        var ok = lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".png") ||
                 lower.EndsWith(".gif") || lower.EndsWith(".webp");
        if (!ok) _logger.LogInformation("Image URL validation failed for {Url}", url);
        return ok;
    }

    private bool ContainTechnologyKeywords(string name)
    {
        var lowered = name.ToLowerInvariant();
        var found = TechKeywords.Any(k => lowered.Contains(k));
        if (!found) _logger.LogInformation("Technology keyword check failed for {Name}", name);
        return found;
    }

    private bool BeAppropriateForHome(string name)
    {
        var lowered = name.ToLowerInvariant();
        foreach (var bad in HomeRestrictedWords)
        {
            if (lowered.Contains(bad.ToLowerInvariant()))
            {
                _logger.LogInformation("Home appropriateness check failed for {Name} (matched {Word})", name, bad);
                return false;
            }
        }
        return true;
    }

    private async Task<bool> PassBusinessRules(CreateProductProfileRequest req, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var addedToday = await _db.Products.CountAsync(p => p.CreatedAt.Date == today, ct);
        if (addedToday >= 500)
        {
            _logger.LogWarning("Daily product addition limit reached: {Count}", addedToday);
            return false;
        }

        if (req.Category == ProductCategory.Electronics && req.Price < 50m)
        {
            _logger.LogWarning("Electronics minimum price rule violated: {Price}", req.Price);
            return false;
        }

        if (req.Category == ProductCategory.Home)
        {
            if (!BeAppropriateForHome(req.Name))
            {
                _logger.LogWarning("Home product content restriction failed for Name:{Name}", req.Name);
                return false;
            }
        }

        if (req.Price > 500m && req.StockQuantity > 10)
        {
            _logger.LogWarning("High value product stock limit violated. Price:{Price} Stock:{Stock}", req.Price, req.StockQuantity);
            return false;
        }

        _logger.LogInformation("Business rules passed for Name:{Name} SKU:{SKU}", req.Name, req.SKU);
        return true;
    }
}
