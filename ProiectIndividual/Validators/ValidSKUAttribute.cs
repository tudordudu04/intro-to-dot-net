using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProiectIndividual.Validators;

public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
{
    public override bool IsValid(object? value)
    {
        var str = (value as string)?.Replace(" ", "") ?? string.Empty;
        if (string.IsNullOrEmpty(str)) return false;
        var regex = new Regex(@"^[A-Za-z0-9\-]{5,20}$");
        return regex.IsMatch(str);
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-validsku", ErrorMessage ?? "Invalid SKU format.");
        MergeAttribute(context.Attributes, "data-val-validsku-pattern", @"^[A-Za-z0-9\-]{5,20}$");
    }

    private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key)) return false;
        attributes.Add(key, value);
        return true;
    }
}