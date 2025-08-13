namespace rinhabackend.Application.Dtos;

public class PaymentEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public double Amount { get; set; }
    public long TimestampMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}