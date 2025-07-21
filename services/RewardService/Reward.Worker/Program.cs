using Microsoft.EntityFrameworkCore;
using Reward.Application.Abstractions.DbContext;
using Reward.Infrastructure;
using Reward.Worker;
using Reward.Worker.Consumers;
using Reward.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<JourneyCreatedConsumer>();
builder.Services.AddHostedService<OutboxPublisherWorker>();
builder.Services.AddHostedService<Worker>();

builder.Services.AddLogging();

var host = builder.Build();
host.Run();
