using FluentValidation;
using Lab03.Requests;

namespace Lab03.Validators;

public class GetBooksSortedByValidator : AbstractValidator<GetBooksSortedByRequest>
{
    public GetBooksSortedByValidator()
    {
        RuleFor(x => x.SortBy)
            .NotEmpty()
            .Must(sortBy => sortBy == "title" || sortBy == "year")
            .WithMessage("SortBy must be either 'title' or 'year'.");
    }
}