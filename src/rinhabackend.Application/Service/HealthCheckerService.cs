using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;
using rinhabackend.Application.Worker;

namespace rinhabackend.Application.Service;

public class HealthCheckerService : IHealthCheckerService
{
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    private const string ProcessorDefault = "http://payment-processor-1:8080/payments/service-health";
    private const string ProcessorFallback = "http://payment-processor-2:8080/payments/service-health";

    private readonly HttpClient _defaultClient;
    private readonly HttpClient _fallbackClient;
    private readonly ILogger<HealthCheckerService> _logger;

    public HealthCheckerService(IPaymentHttpClientFactory factory, ILogger<HealthCheckerService> logger)
    {
        _logger = logger;
        _defaultClient = factory.CreateDefaultClient();
        _fallbackClient = factory.CreateFallbackClient();
    }

    public async Task<string> CheckHealthProcessor()
    {
        if (await IsHealthy(ProcessorDefault))
            return ProcessorDefault;

        await Task.Delay(TimeSpan.FromSeconds(5));
        if (await IsHealthy(ProcessorDefault))
            return ProcessorDefault;

        if (await IsHealthy(ProcessorFallback))
            return ProcessorFallback;

        return "Not healthy";
    }


    private async Task<bool> IsHealthy(string url)
    {
        try
        {
            var client = url.Contains(ProcessorDefault)
                ? _defaultClient
                : _fallbackClient;

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            var healthStatus = JsonSerializer.Deserialize<HealthCheckerResponse>(content, JsonOptions);
            _logger.LogInformation($"Health check response: {healthStatus}");

            return healthStatus is { Failing: false };
        }
        catch
        {
            return false;
        }
    }

}