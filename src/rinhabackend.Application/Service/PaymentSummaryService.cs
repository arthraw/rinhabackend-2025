using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;

namespace rinhabackend.Application.Service;

public class PaymentSummaryService :  IPaymentSummaryService
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<PaymentEntryDto>> _cache = new();
    private readonly ILogger<PaymentSummaryService> _logger;
    
    // implementar redis msm
    
    public PaymentSummaryService(ILogger<PaymentSummaryService> logger)
    {
        _logger = logger;
    }

    public void AddStats(string processor, double amount)
    {
        _logger.LogInformation($"Adding stats: processor={processor}, amount={amount}");

        _cache.AddOrUpdate(
            processor,
            _ => new ConcurrentBag<PaymentEntryDto> { new PaymentEntryDto(DateTime.UtcNow, amount) },
            (_, bag) =>
            {
                bag.Add(new PaymentEntryDto(DateTime.UtcNow, amount));
                return bag;
            }
        );
    }

    public PaymentProcessorDto GetSummary(DateTime dateFrom, DateTime dateTo)
    {
        var defaultData = _cache.GetValueOrDefault("default") ?? new ConcurrentBag<PaymentEntryDto>();
        var fallbackData = _cache.GetValueOrDefault("fallback") ?? new ConcurrentBag<PaymentEntryDto>();
        _logger.LogInformation($"Processadores {defaultData.Count()} e {fallbackData.Count()}");

        return new PaymentProcessorDto
        {
            DefaultProcessor = new PaymentSummaryDto
            {
                totalRequests = defaultData.Count(x => x.Timestamp >= dateFrom && x.Timestamp <= dateTo),
                totalAmount = defaultData
                    .Where(x => x.Timestamp >= dateFrom && x.Timestamp <= dateTo)
                    .Sum(x => x.Amount)
            },
            FallbackProcessor = new PaymentSummaryDto
            {
                totalRequests = fallbackData.Count(x => x.Timestamp >= dateFrom && x.Timestamp <= dateTo),
                totalAmount = fallbackData
                    .Where(x => x.Timestamp >= dateFrom && x.Timestamp <= dateTo)
                    .Sum(x => x.Amount)
            }
        };
    }

}