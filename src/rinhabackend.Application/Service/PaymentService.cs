using System.Net.Http.Json;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;

namespace rinhabackend.Application.Service;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _defaultClient;
    private readonly HttpClient _fallbackClient;
    private readonly IPaymentSummaryService _paymentSummaryService;
    private readonly IPaymentStatsRepository _paymentsStatsRepository;
    public PaymentService(IPaymentHttpClientFactory factory, IPaymentSummaryService paymentSummaryService, IPaymentStatsRepository paymentsStatsRepository)
    {
        _paymentSummaryService = paymentSummaryService;
        _paymentsStatsRepository = paymentsStatsRepository;
        _defaultClient = factory.CreateDefaultClient();
        _fallbackClient = factory.CreateFallbackClient();
    }

    public async Task<PaymentResponse> CreatePayment(PaymentRequestDto paymentDto, string url)
    {
        var client = url == "payment-processor-1" ? _defaultClient : _fallbackClient;

        var payment = new PaymentDto
        {
            correlationId = Guid.NewGuid().ToString(),
            amount = paymentDto.amount,
            date = DateTime.Now
        };

        var result = await client.PostAsJsonAsync("payments", payment);
        var response = await result.Content.ReadFromJsonAsync<PaymentResponse>();
        
        var processorName = url == "payment-processor-1" ? "default" : "fallback";
        
        _paymentSummaryService.AddStats(processorName, payment.amount);
        await _paymentsStatsRepository.IncrementAsync(processorName,  payment.amount);
        return response ?? throw new Exception("Payment response null");
    }

}
