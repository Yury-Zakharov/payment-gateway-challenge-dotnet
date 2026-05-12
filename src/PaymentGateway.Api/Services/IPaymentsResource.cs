using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsResource
{
    Task<PostPaymentResponse?> GetAsync(Guid id);
    Task<PaymentProcessingResult> ProcessPaymentAsync(PostPaymentRequest request);
}