using FluentValidation;
using Lab03.Requests;

namespace Lab03.Validators;

public class GetBooksByAuthorValidator : AbstractValidator<GetBooksByAuthorRequest>
{
    public GetBooksByAuthorValidator()
    {
        RuleFor(x => x.Author).NotEmpty().NotNull();
    }
}