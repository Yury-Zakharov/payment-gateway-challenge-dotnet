using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    Task AddAsync(Payment payment, CancellationToken ct = default);
    Task<Payment?> GetAsync(Guid id, CancellationToken ct = default);
}