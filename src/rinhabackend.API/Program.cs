using rinhabackend.Application.Factory;
using rinhabackend.Application.Interfaces;
using rinhabackend.Application.Service;
using rinhabackend.Application.Worker;
using rinhabackend.Infrastructure.Repository;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

var redisConnectionString = builder.Configuration.GetValue<string>("REDIS_CONNECTION") ?? "localhost:6379";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9999);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddHostedService<PaymentWorker>();

builder.Services.AddControllers();
builder.Services.AddHttpClient("DefaultPayments", client =>
{
    client.BaseAddress = new Uri("http://payment-processor-1:8080/");
});

builder.Services.AddHttpClient("FallbackPayments", client =>
{
    client.BaseAddress = new Uri("http://payment-processor-2:8080/");
});

builder.Services.AddSingleton<IPaymentHttpClientFactory, PaymentHttpClientFactory>();
builder.Services.AddSingleton<IPaymentSummaryService, PaymentSummaryService>();
builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentSummaryService, PaymentSummaryService>();
builder.Services.AddScoped<IHealthCheckerService, HealthCheckerService>();
builder.Services.AddScoped<IPaymentStatsRepository, PaymentStatsRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();

