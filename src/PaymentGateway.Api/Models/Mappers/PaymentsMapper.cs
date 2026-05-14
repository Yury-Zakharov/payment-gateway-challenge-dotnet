using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models.Mappers;

public static class PaymentsMapper
{
    public static GetPaymentResponse ToPaymentResponse(Payment payment) =>
        new()
        {
            Amount = payment.Amount,
            LastFourCardDigits = payment.LastFourCardDigits,
            Currency = payment.Currency,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Id = payment.Id,
            Status = payment.Status
        };
}