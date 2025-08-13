using Microsoft.AspNetCore.Mvc;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;

namespace rinhabackend.API.Controllers;


[ApiController]
[Route("payments-summary")]
public class PaymentSummaryController : ControllerBase
{
    
    private readonly IPaymentSummaryService _paymentSummaryService;
    private readonly IPaymentStatsRepository _paymentStatsRepository;
    public PaymentSummaryController(IPaymentSummaryService paymentSummaryService, IPaymentStatsRepository paymentStatsRepository)
    {
        _paymentSummaryService = paymentSummaryService;
        _paymentStatsRepository = paymentStatsRepository;
    }

    // '/payments-summary?from=2025-08-11T02:14:31.142Z&to=2025-08-11T02:14:44.642Z'
    [HttpGet]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (from >= to)
            return BadRequest("O parâmetro 'from' deve ser menor que 'to'.");

        // var result = _paymentSummaryService.GetSummary(from, to);
        var result = await _paymentStatsRepository.GetStatsAsync(from, to);
        var response = new
        {
            @default = new
            {
                totalRequests = result.DefaultProcessor.TotalRequests,
                totalAmount = result.DefaultProcessor.TotalAmount,
            },
            fallback = new
            {
                totalRequests = result.FallbackProcessor.TotalRequests,
                totalAmount = result.FallbackProcessor.TotalAmount
            }
        };

        return Ok(response);
    }
    
    [HttpPost("add")]
    public IActionResult AddStats([FromBody] AddStatsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Processor) ||
            (request.Processor != "default" && request.Processor != "fallback"))
        {
            return BadRequest("Processor deve ser 'default' ou 'fallback'");
        }

        if (request.Amount <= 0)
        {
            return BadRequest("Amount deve ser maior que zero");
        }

        _paymentSummaryService.AddStats(request.Processor, request.Amount);

        return Ok(new { message = "Stats adicionadas com sucesso!" });
    }

}

public class AddStatsRequest
{
    public string Processor { get; set; }
    public double Amount { get; set; }
}