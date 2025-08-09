using Microsoft.AspNetCore.Mvc;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Service;

namespace rinhabackend.API.Controllers;

[ApiController]
[Route("[controller]")] // favor verificar se essa eh a rota msm valeu?
public class PaymentController : ControllerBase
{
    
    private readonly PaymentService _service;

    public PaymentController(PaymentService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult PostPayment([FromBody] PaymentDto payment)
    {
        if (payment.amount <= 0)
        {
            return BadRequest(
                new { message = payment }
            );
        }

        try
        {
            var response = _service.createPayment(payment);
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}