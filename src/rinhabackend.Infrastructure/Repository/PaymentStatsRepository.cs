using System.Text.Json;
using rinhabackend.Application.Dtos;
using rinhabackend.Application.Interfaces;
using StackExchange.Redis;

namespace rinhabackend.Infrastructure.Repository;

public class PaymentStatsRepository : IPaymentStatsRepository
{
    private readonly IDatabase _db;
    private const string MetricsKey = "payment_stats";

    public PaymentStatsRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task IncrementAsync(string processor, double amount)
    {
        processor = processor.ToLowerInvariant();
        if (processor != "default" && processor != "fallback")
            throw new ArgumentException("Processor inválido. Use 'default' ou 'fallback'.");

        var key = $"payment:stats:{processor}";

        var entry = new PaymentEntry
        {
            Amount = amount,
        };

        var score = (double)entry.TimestampMs;
        var member = (RedisValue)JsonSerializer.Serialize(entry);

        await _db.SortedSetAddAsync(key, member, score);
    }

    // public async Task<PaymentProcessor?> GetStatsAsync(DateTime from, DateTime to)
    // {
    //     string? json = await _db.StringGetAsync(MetricsKey);
    //
    //     return json != null
    //         ? JsonSerializer.Deserialize<PaymentProcessor>(json)
    //         : null;
    // }
    
    public async Task<PaymentProcessor?> GetStatsAsync(DateTime from, DateTime to)
    {
        var fromScore = (double)new DateTimeOffset(from).ToUnixTimeMilliseconds();
        var toScore   = (double)new DateTimeOffset(to).ToUnixTimeMilliseconds();

        var def = await _db.SortedSetRangeByScoreAsync("payment:stats:default", fromScore, toScore);
        var fbk = await _db.SortedSetRangeByScoreAsync("payment:stats:fallback", fromScore, toScore);

        static (int count, double sum) Aggregate(RedisValue[] vals)
        {
            var total = 0.0;
            var count = 0;
            foreach (var v in vals)
            {
                var e = JsonSerializer.Deserialize<PaymentEntry>(v!);
                if (e is null) continue;
                total += e.Amount;
                count++;
            }
            return (count, total);
        }

        var (defCount, defSum) = Aggregate(def);
        var (fbkCount, fbkSum) = Aggregate(fbk);

        return new PaymentProcessor
        {
            DefaultProcessor = new PaymentStats { TotalRequests = defCount, TotalAmount = defSum },
            FallbackProcessor = new PaymentStats { TotalRequests = fbkCount, TotalAmount = fbkSum }
        };
    }

}