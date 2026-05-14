using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public readonly record struct BankPaymentRequest
{
    [JsonPropertyName("card_number")]
    public required string CardNumber { get; init; }

    [JsonPropertyName("expiry_date")]
    public required string ExpiryDate { get; init; }   // "MM/YYYY"

    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    [JsonPropertyName("amount")]
    public required long Amount { get; init; }

    [JsonPropertyName("cvv")]
    public required string Cvv { get; init; }
}