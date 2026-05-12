using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PaymentGateway.Api.Validation;

public static class ModelValidator
{
    public static IList<ValidationResult> Validate<T>([NotNull] T model) 
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        return results;
    }
}