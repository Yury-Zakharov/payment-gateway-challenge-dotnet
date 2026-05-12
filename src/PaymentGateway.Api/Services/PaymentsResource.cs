using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsResource(PaymentsRepository paymentsRepository) : IPaymentsResource
{
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;

    async Task<PostPaymentResponse?> IPaymentsResource.GetAsync(Guid id)
    {
        return await Task.FromResult(_paymentsRepository.Get(id));
    }

    async Task<PaymentProcessingResult> IPaymentsResource.ProcessPaymentAsync(PostPaymentRequest request)
    {
        throw new NotImplementedException();
    }
}