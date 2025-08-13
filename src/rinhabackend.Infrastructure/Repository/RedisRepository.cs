using System.Text.Json;
using rinhabackend.Application.Dtos;
using rinhabackend.Application.DTOs;
using rinhabackend.Application.Interfaces;
using StackExchange.Redis;

namespace rinhabackend.Infrastructure.Repository;

public class RedisRepository : IRedisRepository
{
 
    private readonly IConnectionMultiplexer _redis;
    private const string QueueKey = "paymentQueue";


    public RedisRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    
    public async Task Enqueue(string message)
    {
        await _redis.GetDatabase().ListRightPushAsync(QueueKey, message);
    }

    public async Task<string?> Dequeue()
    {
        var result = await _redis.GetDatabase().ListLeftPopAsync(QueueKey);
        return result.HasValue ? result.ToString() : null;
    }

    public string? DequeueBlocking(int timeoutSeconds = 5)
    {
        var result = _redis.GetDatabase().ListLeftPop(QueueKey);
        if (result.HasValue)
            return result.ToString();

        var blpopResult = _redis.GetDatabase().ListLeftPop(QueueKey);
        return blpopResult.HasValue ? blpopResult.ToString() : null;
    }
    
    
}

