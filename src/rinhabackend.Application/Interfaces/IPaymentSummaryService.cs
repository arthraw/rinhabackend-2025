using rinhabackend.Application.DTOs;

namespace rinhabackend.Application.Interfaces;

public interface IPaymentSummaryService
{
    void AddStats(string processor, double amount);
    PaymentProcessorDto GetSummary(DateTime dateFrom, DateTime dateTo);
}