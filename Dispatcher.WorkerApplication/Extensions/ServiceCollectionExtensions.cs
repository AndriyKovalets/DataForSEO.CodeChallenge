using Dispatcher.WorkerApplication.Abstractions.Services;
using Dispatcher.WorkerApplication.Abstractions.Services.Parsers;
using Dispatcher.WorkerApplication.Abstractions.Services.Processors;
using Dispatcher.WorkerApplication.Parsers;
using Dispatcher.WorkerApplication.Processors;
using Dispatcher.WorkerApplication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.WorkerApplication.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddWorkerApplicationServices(this IServiceCollection services)
    {
        services.AddServices();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IDispatcherWorkerService, DispatcherWorkerService>();
        services.AddScoped<IMetricProcessorsModule, MetricProcessorsModule>();
        services.AddScoped<IParserFactory, ParserFactory>();
    }
}