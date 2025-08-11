using rinhabackend.Application.Interfaces;

namespace rinhabackend.Application.Factory;

public class PaymentHttpClientFactory : IPaymentHttpClientFactory
{
    private readonly IHttpClientFactory _factory;

    public PaymentHttpClientFactory(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public HttpClient CreateDefaultClient()
    {
        var client = _factory.CreateClient("DefaultPayments");
        client.BaseAddress = new Uri("http://payment-processor-1:8080/");
        return client;
    }

    public HttpClient CreateFallbackClient()
    {
        var client = _factory.CreateClient("FallbackPayments");
        client.BaseAddress = new Uri("http://payment-processor-2:8080/");
        return client;
    }
}