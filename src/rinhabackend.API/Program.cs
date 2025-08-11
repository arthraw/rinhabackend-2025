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
    options.ListenAnyIP(8080);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddHostedService<PaymentWorker>();

builder.Services.AddControllers();
builder.Services.AddHttpClient("DefaultPayments", client =>
{
    client.BaseAddress = new Uri("http://api1:8080/");
});

builder.Services.AddHttpClient("FallbackPayments", client =>
{
    client.BaseAddress = new Uri("http://api2:8080/");
});

builder.Services.AddSingleton<IPaymentHttpClientFactory, PaymentHttpClientFactory>();
builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IHealthCheckerService, HealthCheckerService>();

var app = builder.Build();

app.MapControllers();

app.Run();

