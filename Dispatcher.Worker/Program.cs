using Dispatcher.Application.Extensions;
using Dispatcher.Domain;
using Dispatcher.Infrastructure;
using Dispatcher.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWorkerDomain(builder.Configuration);

var host = builder.Build();
host.Run();