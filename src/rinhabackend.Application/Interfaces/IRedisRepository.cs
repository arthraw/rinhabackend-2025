namespace rinhabackend.Application.Interfaces;

public interface IRedisRepository
{
    void Enqueue(string name);
    string? Dequeue();
    string? DequeueBlocking(int timeoutSeconds = 5);
}