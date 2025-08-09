using rinhabackend.Application.Interfaces;
using StackExchange.Redis;

namespace rinhabackend.Infrastructure.Repository;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _db;
    private readonly string _queueKey;

    public RedisRepository(IConnectionMultiplexer redis, string queueKey)
    {
        _db = redis.GetDatabase();
        _queueKey = queueKey;
    }

    public void Enqueue(string message)
    {
        _db.ListRightPush(_queueKey, message);
    }

    public string? Dequeue()
    {
        var result = _db.ListLeftPop(_queueKey);
        return result.HasValue ? result.ToString() : null;
    }

    public string? DequeueBlocking(int timeoutSeconds = 5)
    {
        var result = _db.ListLeftPop(_queueKey);
        if (result.HasValue)
            return result.ToString();

        var blpopResult = _db.ListLeftPop(_queueKey);
        return blpopResult.HasValue ? blpopResult.ToString() : null;
    }    
    
    
}

