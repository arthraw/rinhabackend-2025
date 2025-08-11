namespace rinhabackend.Application.Interfaces;

public interface IHealthCheckerService
{
    Task<string> CheckHealthProcessor();
}