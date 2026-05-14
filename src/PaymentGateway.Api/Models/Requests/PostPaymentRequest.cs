using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Requests;

public record  PostPaymentRequest : IValidatableObject
{
    /// <summary>
    /// Card number.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MinLength(14, ErrorMessage = "Card number must be at least 14 characters long.")]
    [MaxLength(19, ErrorMessage = "Card number must be at most 19 characters long.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Card number must only contain numeric characters.")]
    public string CardNumber { get; init; } = string.Empty;
    
    /// <summary>
    /// Expiry month.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Range(1, 12)]
    public int ExpiryMonth { get; init; }
    
    /// <summary>
    /// Expiry year.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Range(2000, 2100)] // I'm sure this coding challenge is not going to survive 2100.
    public int ExpiryYear { get; init; }
    
    /// <summary>
    /// Currency code.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public Currency Currency { get; init; }
    
    /// <summary>
    /// Payment amount in minor currency units.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public long Amount { get; init; }

    /// <summary>
    /// Security code.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MinLength(3, ErrorMessage = "CVV must be at least 3 characters long.")]
    [MaxLength(4, ErrorMessage = "CVV must be at most 4 characters long.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "CVV must only contain numeric characters.")]
    public string Cvv { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // For better testability I suggest to use date-time provider.
        var now = DateTime.UtcNow;
        
        // Need clarification when card is considered expired - actual expiry month or the month after. 
        if (ExpiryYear < now.Year || (ExpiryYear == now.Year && ExpiryMonth <= now.Month))
            yield return new ValidationResult("Value must be in the future.", 
                [nameof(ExpiryMonth), nameof(ExpiryYear)]);
        
        if (!Enum.IsDefined(typeof(Currency), Currency))
            yield return new ValidationResult("The value is not a valid currency code.", 
                [nameof(Currency)]);
    }
}