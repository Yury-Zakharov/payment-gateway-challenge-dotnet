namespace PaymentGateway.Api.Models.Responses;

public sealed record Payment(
    string Id,         
    string Status,                
    string LastFourCardDigits,
    int ExpiryMonth,
    int ExpiryYear,
    Currency Currency,
    long Amount
);