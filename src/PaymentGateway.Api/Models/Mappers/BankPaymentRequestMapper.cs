using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Models.Mappers;

public static class BankPaymentRequestMapper
{
    public static BankPaymentRequest ToBankPayment(PostPaymentRequest request) =>
        new()
        {
            Amount = request.Amount,
            CardNumber = request.CardNumber,
            ExpiryDate = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
            Currency = request.Currency.ToString(),
            Cvv = request.Cvv
        };

    public static PostPaymentResponse ToBankResponse(Payment payment) =>
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