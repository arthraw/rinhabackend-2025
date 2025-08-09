using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using rinhabackend.Application.DTOs;

namespace rinhabackend.API.Controllers;

[ApiController]
[Route("[controller]")] // favor verificar se essa eh a rota msm valeu?
public class PaymentController : ControllerBase
{
    [HttpPost]
    public IActionResult PostPayment([FromBody] PaymentDto payment)
    {
        if (payment.amount <= 0)
        {
            return BadRequest(
                new { message = payment }
            );
        }

        return Ok(new { message = "Pagamento processado com sucesso", payment = payment });
    }
}