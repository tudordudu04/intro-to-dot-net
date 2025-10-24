using FluentValidation;
using ProiectIndividual.Products;

namespace ProiectIndividual.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductProfileRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required");
        RuleFor(x => x.Price).NotEmpty().NotNull().WithMessage("Price is required");
        RuleFor(x => x.SKU).NotEmpty().NotNull().WithMessage("SKU is required");
        RuleFor(x => x.Category).IsInEnum().WithMessage("Category is required");
        RuleFor(x => x.StockQuantity)
            .GreaterThan(0)
            .WithMessage("Stock quantity must be greater than zero");
        RuleFor(x => x.ReleaseDate).NotEmpty().NotNull().WithMessage("Release date is required");
        RuleFor(x => x.ImageUrl).NotEmpty().NotNull().WithMessage("Image url is required");
        RuleFor(x => x.Brand).NotEmpty().NotNull().WithMessage("Brand is required");
        
    }
}