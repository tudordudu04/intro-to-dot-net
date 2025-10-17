using FluentValidation;
using Lab03.Requests;

namespace Lab03.Validators;

public class GetBooksByPaginationValidator : AbstractValidator<GetBooksByPaginationRequest>
{
    public GetBooksByPaginationValidator()
    {
        RuleFor(x => x.Page).NotEmpty().NotNull().GreaterThan(0);
        RuleFor(x => x.PageSize).NotEmpty().NotNull().GreaterThan(0);
    }
}