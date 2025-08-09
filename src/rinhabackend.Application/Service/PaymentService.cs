using System.Net.Http.Json;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;

namespace rinhabackend.Application.Service;

public class PaymentService :  IPaymentService
{
    private readonly HttpClient _http;

    private readonly string _paymentUrl;

    public PaymentService(HttpClient http)
    {
        _paymentUrl = "http://localhost:8001/payments";
        this._http = http;
    }
    
    public async Task<PaymentResponse> createPayment(PaymentDto paymentDto)
    {
        
        
        
        var result = await _http.PostAsJsonAsync(_paymentUrl, paymentDto);
        var response = await result.Content.ReadFromJsonAsync<PaymentResponse>();
        return response ?? throw new Exception("Payment response null");
    }
}