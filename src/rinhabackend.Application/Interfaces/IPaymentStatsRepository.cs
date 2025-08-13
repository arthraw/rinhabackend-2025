using rinhabackend.Application.Dtos;

namespace rinhabackend.Application.Interfaces;

public interface IPaymentStatsRepository
{
    Task IncrementAsync(string processor, double amount);
    Task<PaymentProcessor?> GetStatsAsync(DateTime from, DateTime to);
}