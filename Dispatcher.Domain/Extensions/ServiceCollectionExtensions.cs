using Dispatcher.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDomainOptions(configuration);
    }
    
    public static void AddWorkerDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDomainOptions(configuration);
        services.AddWorkerDomainOptions(configuration);
    }

    private static void AddDomainOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QueueNameOptions>(
            configuration.GetSection(QueueNameOptions.SectionName));
    }
    
    private static void AddWorkerDomainOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<WorkerOptions>(
            configuration.GetSection(WorkerOptions.SectionName));
    }
}