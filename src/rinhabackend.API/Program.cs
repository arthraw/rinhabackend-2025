using rinhabackend.Application.Interfaces;
using rinhabackend.Application.Service;
using rinhabackend.Application.Worker;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

var redisConnectionString = builder.Configuration.GetValue<string>("REDIS_CONNECTION") ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddHostedService<PaymentWorker>();

builder.Services.AddControllers();
builder.Services.AddScoped<IPaymentService, PaymentService>();
var app = builder.Build();

app.UseHttpsRedirection();

app.Run();

