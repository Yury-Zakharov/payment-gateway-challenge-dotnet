using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public sealed record Payment(
    Guid Id,         
    PaymentStatus Status,                
    string LastFourCardDigits,
    int ExpiryMonth,
    int ExpiryYear,
    Currency Currency,
    long Amount,
    string? AuthorizationCode
);