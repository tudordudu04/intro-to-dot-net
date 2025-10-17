using FluentValidation;
using Lab03.Requests;

namespace Lab03.Validators;

public class UpdateBookValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull().GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().NotNull();
        RuleFor(x => x.Author).NotEmpty().NotNull();
        RuleFor(x => x.Year).NotEmpty().NotNull();
    }
}