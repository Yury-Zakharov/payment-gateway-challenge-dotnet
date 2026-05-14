namespace PaymentGateway.Api.Models.Responses;

// Poor man's Discriminated union.
public abstract record PaymentProcessingResult
{
    private PaymentProcessingResult() { }

    /// <summary>
    /// Bank authorized payment.
    /// </summary>
    public sealed record Authorized(PostPaymentResponse Response) : PaymentProcessingResult;

    /// <summary>
    /// Bank declined payment.
    /// </summary>
    public sealed record Declined(PostPaymentResponse Response) : PaymentProcessingResult;

    /// <summary>
    /// Bank returned 503 or network error.
    /// </summary>
    public sealed record BankError(string Message) : PaymentProcessingResult;
}