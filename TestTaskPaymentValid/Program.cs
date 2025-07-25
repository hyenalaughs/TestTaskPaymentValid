using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TestTaskPaymentValid.Application.Services;
using TestTaskPaymentValid.Domain.Interfaces;
using TestTaskPaymentValid.Infrastructure.DAL.Core;
using TestTaskPaymentValid.Infrastructure.DAL.Interfaces;
using TestTaskPaymentValid.Infrastructure.DAL.Repositories;
using TestTaskPaymentValid.Infrastructure.Gateways;
using TestTaskPaymentValid.Infrastructure.Interfaces;
using TestTaskPaymentValid.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//using (var scope = builder.Services.BuildServiceProvider().CreateScope())
//{
//    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
//    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

//    await DbInitializer.InitializeDatabaseAsync(config, logger);
//    await DbInitializer.InitializeAsync(config, logger);
//}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisHost = builder.Configuration["Redis:Host"] ?? "localhost";
    var redisPort = builder.Configuration["Redis:Port"] ?? "6379";
    var config = ConfigurationOptions.Parse($"{redisHost}:{redisPort}", true);
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddSingleton<ILoggerProvider>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return new LoggerProvider(redis.GetDatabase());
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddScoped<IPaymentValidator, PaymentValidator>();
builder.Services.AddScoped<IRetryService, RetryService>();
builder.Services.AddScoped<IGatewayRouter, GatewayRouter>();
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IKeyRepository, KeyRepository>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();

// fake gareways 
builder.Services.AddSingleton<IPaymentGateway, AGateway>();
builder.Services.AddSingleton<IPaymentGateway, BGateway>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
loggerFactory.AddProvider(new LoggerProvider(redis.GetDatabase())); 

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await DbInitializer.InitializeDatabaseAsync(config, logger);
    await DbInitializer.InitializeAsync(config, logger);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
