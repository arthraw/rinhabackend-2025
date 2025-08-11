using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;
using rinhabackend.Application.Service;
using rinhabackend.Infrastructure.Repository;

namespace rinhabackend.API.Controllers;

[ApiController]
[Route("payments")]
public class PaymentController : ControllerBase
{
    private readonly IRedisRepository _redisRepository;
    
    public PaymentController(IRedisRepository redisRepository)
    {
        _redisRepository = redisRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok("API funcionando");

    [HttpPost]
    public async Task<IActionResult> PostPayment([FromBody] PaymentRequestDto payment)
    {
        if (payment.amount <= 0)
        {
            return BadRequest(new { message = "Valor inválido" });
        }
    
        try
        {
            var jsonMessage = JsonSerializer.Serialize(payment);
    
            await _redisRepository.Enqueue(jsonMessage);
            return Accepted(new { message = "Pagamento recebido" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = $"Erro ao enfileirar: {e.Message}" });
        }
    }

    [HttpPost("teste")]
    public async Task<IActionResult> PostTeste()
    {
        return Ok("teste");
    }


}