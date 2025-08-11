using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace rinhabackend.Application.Worker;

public class PaymentWorker : BackgroundService
{
    private readonly IRedisRepository _redisRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentWorker> _logger;
    private readonly IHealthCheckerService _healthCheckerService;
    public PaymentWorker(
        IRedisRepository redisRepository,
        IPaymentService paymentService,
        ILogger<PaymentWorker> logger, IHealthCheckerService healthCheckerService)
    {
        _redisRepository = redisRepository;
        _paymentService = paymentService;
        _logger = logger;
        _healthCheckerService = healthCheckerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de pagamentos iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _redisRepository.Dequeue();

                if (message != null)
                {
                    var payment = JsonSerializer.Deserialize<PaymentRequestDto>(message);
                    _logger.LogInformation($"Pagamento: {payment}");
                    
                    if (payment != null)
                    {
                        _logger.LogInformation($"Processando pagamento: {payment.amount}");

                        string url;
                        do
                        {
                            url = await _healthCheckerService.CheckHealthProcessor();

                            if (url == "Not healthy")
                            {
                                _logger.LogWarning("Nenhum proce    ssador disponível. Aguardando 5 segundos para tentar novamente...");
                                await Task.Delay(5000, stoppingToken);
                            }

                        } while (url == "Not healthy" && !stoppingToken.IsCancellationRequested);

                        if (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogInformation($"Processador utilizado: {url}");
                            await _paymentService.CreatePayment(payment, url);
                        }
                    }
                }
                else
                {
                    await Task.Delay(500, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pagamento");
                await Task.Delay(1000, stoppingToken);
            }
        }

        _logger.LogInformation("Worker de pagamentos finalizado.");
    }

}
