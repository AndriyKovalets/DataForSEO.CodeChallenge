using Dispatcher.Domain;
using Dispatcher.Domain.Extensions;
using Dispatcher.Infrastructure;
using Dispatcher.Infrastructure.Extensions;
using Dispatcher.Worker;
using Dispatcher.WorkerApplication.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddWorkerApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWorkerDomain(builder.Configuration);

var host = builder.Build();
host.Run();