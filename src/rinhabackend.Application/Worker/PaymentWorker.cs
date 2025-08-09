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

    public PaymentWorker(
        IRedisRepository redisRepository,
        IPaymentService paymentService,
        ILogger<PaymentWorker> logger)
    {
        _redisRepository = redisRepository;
        _paymentService = paymentService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de pagamentos iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = _redisRepository.Dequeue();

                if (message != null)
                {
                    var payment = JsonSerializer.Deserialize<PaymentDto>(message);

                    if (payment != null)
                    {
                        _logger.LogInformation($"Processando pagamento: {payment.amount}");
                        await _paymentService.createPayment(payment);
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
