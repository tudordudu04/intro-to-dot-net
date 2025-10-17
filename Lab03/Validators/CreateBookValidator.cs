using FluentValidation;
using Lab03.Requests;

namespace Lab03.Validators;

public class CreateBookValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().NotNull();
        RuleFor(x => x.Author).NotEmpty().NotNull();
        RuleFor(x => x.Year).NotEmpty().NotNull();
    }
}