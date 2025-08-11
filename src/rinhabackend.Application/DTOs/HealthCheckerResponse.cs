namespace rinhabackend.Application.DTOs;

public record HealthCheckerResponse()
{
    public bool Failing { get; set; }
    public int MinResponseTime { get; set; }
};