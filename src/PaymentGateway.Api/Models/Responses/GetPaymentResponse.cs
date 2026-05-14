using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public readonly record struct GetPaymentResponse
{
    public Guid Id { get; init; }
    public PaymentStatus Status { get; init; }
    public string LastFourCardDigits { get; init; }
    public int ExpiryMonth { get; init; }
    public int ExpiryYear { get; init; }
    public Currency Currency { get; init; }
    public long Amount { get; init; }
}