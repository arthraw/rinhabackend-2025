namespace rinhabackend.Application.DTOs;

public record PaymentRequestDto()
{
    public decimal amount { get; set; }
};