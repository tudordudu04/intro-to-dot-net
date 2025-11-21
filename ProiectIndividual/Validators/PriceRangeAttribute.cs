using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProiectIndividual.Validators;

public class PriceRangeAttribute(double min, double max) : ValidationAttribute
{
    private readonly decimal _min = Convert.ToDecimal(min);
    private readonly decimal _max = Convert.ToDecimal(max);

    public override bool IsValid(object? value)
    {
        if (value == null) return false;
        if (!decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var price)) return false;
        return price >= _min && price <= _max;
    }

    public override string FormatErrorMessage(string name)
    {
        return ErrorMessage ?? $"{name} must be between {_min:C} and {_max:C}.";
    }
}