using CorrelationId;
using CorrelationId.DependencyInjection;
using HealthChecks.UI.Client;
using Journey.API.Extensions;
using Journey.API.Middleware;
using Journey.API.Services;
using Journey.API.Workers;
using Journey.Application;
using Journey.Domain.Abstractions.Interface;
using Journey.Infrastructure;
using Journey.Infrastructure.Extensions;
using Journey.Infrastructure.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(); 

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddSwaggerAuth());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IOutboxPublisher, RabbitMQPublisher>();
builder.Services.AddHostedService<OutboxPublisherWorker>();

builder.Services.AddHostedService<Journey.API.Consumers.DailyGoalAchievedConsumer>();

builder.Services.AddSignalR();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IActualUser, ActualUser>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCustomMiddlewares();

app.MapControllers();
app.MapHealthChecks("/healthz");
app.MapHealthChecks("/readyz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.RunStartupTasksAsync();

app.Run();