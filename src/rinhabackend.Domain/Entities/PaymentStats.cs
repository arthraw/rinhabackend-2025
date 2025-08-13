using System.Text.Json.Serialization;

namespace rinhabackend.Application.Dtos;

public class PaymentStats
{
    [JsonPropertyName("totalRequests")]
    public int TotalRequests { get; set; }
    
    [JsonPropertyName("totalAmount")]
    public double TotalAmount { get; set; }
}