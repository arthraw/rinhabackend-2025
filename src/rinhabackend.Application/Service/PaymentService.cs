using System.Net.Http.Json;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;

namespace rinhabackend.Application.Service;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _defaultClient;
    private readonly HttpClient _fallbackClient;

    public PaymentService(IPaymentHttpClientFactory factory)
    {
        _defaultClient = factory.CreateDefaultClient();
        _fallbackClient = factory.CreateFallbackClient();
    }

    public async Task<PaymentResponse> CreatePayment(PaymentRequestDto paymentDto, string url)
    {
        var client = url.Contains("payment-processor-1") ? _defaultClient : _fallbackClient;

        var payment = new PaymentDto
        {
            correlationId = Guid.NewGuid().ToString(),
            amount = paymentDto.amount,
            date = DateTime.Now
        };

        var result = await client.PostAsJsonAsync(url, payment);
        var response = await result.Content.ReadFromJsonAsync<PaymentResponse>();

        return response ?? throw new Exception("Payment response null");
    }
}
