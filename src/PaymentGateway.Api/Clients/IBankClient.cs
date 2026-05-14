using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Clients;

public interface IBankClient
{
    Task<BankPaymentResponse> PostPaymentAsync(BankPaymentRequest request, CancellationToken ct = default);
}