namespace rinhabackend.Application.DTOs;

public record PaymentSummaryDto()
{
    public int totalRequests { get; set; }
    public double totalAmount { get; set; }
};
