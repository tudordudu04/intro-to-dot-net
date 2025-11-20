using System.ComponentModel.DataAnnotations;

namespace ProiectIndividual.Validators;

public class ProductCategoryAttribute(params object[] allowedCategories) : ValidationAttribute
{
    private readonly Array _allowed = allowedCategories;

    public override bool IsValid(object? value)
    {
        if (value == null) return false;
        return _allowed.Cast<object>().Any(a => a.Equals(value));
    }

    public override string FormatErrorMessage(string name)
    {
        var list = string.Join(", ", _allowed.Cast<object>());
        return ErrorMessage ?? $"{name} must be one of the following categories: {list}.";
    }
}