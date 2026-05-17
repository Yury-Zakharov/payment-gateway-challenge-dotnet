using PaymentGateway.Api.Clients;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public sealed class PaymentsResource(IPaymentsRepository paymentsRepository, IBankClient bankClient, ILogger<PaymentsResource> logger) : IPaymentsResource
{
    async Task<GetPaymentResponse?> IPaymentsResource.GetAsync(Guid id, CancellationToken ct)
    {
        var payment = await paymentsRepository.GetAsync(id, ct);
        return payment is null
            ? null
            : PaymentsMapper.ToPaymentResponse(payment);
    }

    async Task<PaymentProcessingResult> IPaymentsResource.ProcessPaymentAsync(PostPaymentRequest request, CancellationToken ct)
    {
        var bankRequest = BankPaymentRequestMapper.ToBankPayment(request);
        BankPaymentResponse bankResponse;

        try
        {
            bankResponse = await bankClient.PostPaymentAsync(bankRequest, ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {
            return new PaymentProcessingResult.BankError("Bank is unavailable");
        }
        catch (Exception ex)
        {
            return new PaymentProcessingResult.BankError(ex.Message);
        }
        
        var payment = new Payment(
            Id: Guid.NewGuid(),
            Status: bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
            LastFourCardDigits: request.CardNumber.Length >= 4
                ? request.CardNumber[^4..]
                : request.CardNumber,
            ExpiryMonth: request.ExpiryMonth,
            ExpiryYear: request.ExpiryYear,
            Currency: request.Currency,
            Amount: request.Amount,
            AuthorizationCode: bankResponse.AuthorizationCode
        );
        
        await paymentsRepository.AddAsync(payment, ct);
        PostPaymentResponse response = BankPaymentRequestMapper.ToBankResponse(payment);
        
        logger.LogInformation("Payment processed {Status} {Id}", payment.Status, payment.Id);
        
        return bankResponse.Authorized
            ? new PaymentProcessingResult.Authorized(response)
            : new PaymentProcessingResult.Declined(response);
    }
}