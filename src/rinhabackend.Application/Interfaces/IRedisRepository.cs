namespace rinhabackend.Application.Interfaces;

public interface IRedisRepository
{
    Task Enqueue(string name);
    Task<string?> Dequeue();
    string? DequeueBlocking(int timeoutSeconds = 5);
}