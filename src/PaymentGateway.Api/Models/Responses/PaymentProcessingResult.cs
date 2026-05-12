namespace PaymentGateway.Api.Models.Responses;

public abstract record PaymentProcessingResult
{
    private PaymentProcessingResult() { }

    /// <summary>
    /// Bank authorized (odd last digit). Store + return 201.
    /// </summary>
    public sealed record Authorized(Payment Payment) : PaymentProcessingResult;

    /// <summary>
    /// Bank declined (even last digit). Store + return 201.
    /// </summary>
    public sealed record Declined(Payment Payment) : PaymentProcessingResult;

    /// <summary>
    /// Bank returned 503 (card ends with 0) or network error.
    /// Do NOT store. Return 503.
    /// </summary>
    public sealed record BankError(string Message) : PaymentProcessingResult;
}