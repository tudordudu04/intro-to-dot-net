using FluentValidation;
using ProiectIndividual.Requests;

namespace ProiectIndividual.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductProfileRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100).WithMessage("Brand must not exceed 100 characters.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(64).WithMessage("SKU must not exceed 64 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category must be a valid ProductCategory.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0m).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.ReleaseDate)
            .Must(BeAValidDate).WithMessage("ReleaseDate must be a valid date (not the default DateTime).");

        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .Must(BeAValidUri).When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("ImageUrl must be a valid absolute URL starting with http:// or https://.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("StockQuantity cannot be negative.");
        }
    private static bool BeAValidDate(DateTime date)
    {
        return date != default && date.Kind != DateTimeKind.Unspecified;
    }

    private static bool BeAValidUri(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}