namespace rinhabackend.Application.Interfaces;

public interface IPaymentHttpClientFactory
{
    HttpClient CreateDefaultClient();
    HttpClient CreateFallbackClient();
}