using System.Text.Json.Serialization;

namespace rinhabackend.Application.DTOs;

public class PaymentProcessorDto
{
    [JsonPropertyName("default")]
    public PaymentSummaryDto DefaultProcessor { get; set; }

    [JsonPropertyName("fallback")]
    public PaymentSummaryDto FallbackProcessor { get; set; }
}