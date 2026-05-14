using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public sealed class InMemoryPaymentRepository : IPaymentsRepository
{
    private readonly List<Payment> _payments = [];

    Task IPaymentsRepository.AddAsync(Payment payment, CancellationToken ct)
    {
        _payments.Add(payment);
        return Task.CompletedTask;
    }

    Task<Payment?> IPaymentsRepository.GetAsync(Guid id, CancellationToken ct) => 
        Task.FromResult( _payments.FirstOrDefault(p => p.Id == id));
}