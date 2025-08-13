namespace rinhabackend.Application.DTOs;

public class PaymentDto()
{
    public required string correlationId { get; set; }
    public required double amount { get; set; }
    public required DateTime date { get; set; }
};