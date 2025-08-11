namespace rinhabackend.Application.Dtos;

public record Payment
{
    public required string correlationId { get; set; }
    public required decimal amount { get; set; }
    public required DateTime date { get; set; }
}