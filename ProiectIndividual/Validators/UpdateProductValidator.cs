using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProiectIndividual.Enums;
using ProiectIndividual.Persistance;
using ProiectIndividual.Requests;

namespace ProiectIndividual.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator(ProductManagementContext context)
    {
        RuleFor(x => x.Id)
            .NotNull().NotEmpty()
            .NotEqual(Guid.Empty).WithMessage("Id must be a valid GUID.")
            .MustAsync(async (id, cancellation) =>
            {
                if (id == Guid.Empty) return false;
                return await context.Products.AnyAsync(p => p.Id == id, cancellation);
            })
            .WithMessage("Product with the specified ID does not exist.");
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null)
            .WithMessage("Name is required");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.SKU)
            .NotEmpty()
            .When(x => x.SKU != null)
            .WithMessage("SKU is required");

        RuleFor(x => x.Category)
            .Must(c => Enum.IsDefined(typeof(ProductCategory), c.Value))
            .When(x => x.Category.HasValue)
            .WithMessage("Category is invalid");

        RuleFor(x => x.StockQuantity)
            .GreaterThan(0)
            .When(x => x.StockQuantity.HasValue)
            .WithMessage("Stock quantity must be greater than zero");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty()
            .When(x => x.ReleaseDate.HasValue)
            .WithMessage("Release date is required");

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .When(x => x.ImageUrl != null)
            .WithMessage("Image url is required");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .When(x => x.Brand != null)
            .WithMessage("Brand is required");
    }
}