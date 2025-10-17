using FluentValidation;
using Lab03.Requests;

namespace Lab03.Validators;

public class GetBookByIdValidator : AbstractValidator<GetBookByIdRequest>
{
    public GetBookByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull().GreaterThan(0);
    }
}