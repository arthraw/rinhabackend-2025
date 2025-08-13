namespace rinhabackend.Application.Dtos;

public class PaymentProcessor
{
    public PaymentStats DefaultProcessor { get; set; }

    public PaymentStats FallbackProcessor { get; set; }
}