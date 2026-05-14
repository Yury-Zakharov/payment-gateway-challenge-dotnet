using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsResource
{
    Task<GetPaymentResponse?> GetAsync(Guid id, CancellationToken ct = default);
    Task<PaymentProcessingResult> ProcessPaymentAsync(PostPaymentRequest request, CancellationToken ct = default);
}