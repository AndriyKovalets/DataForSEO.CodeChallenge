using Dispatcher.Application.Abstractions.Services;
using Dispatcher.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddServices();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IDispatcherService, DispatcherService>();
    }
    
}