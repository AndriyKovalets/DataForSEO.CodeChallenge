using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Infrastructure.Persistence;
using Dispatcher.Infrastructure.Queue;
using Dispatcher.SharedApplication.Abstractions.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Dispatcher.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddDbRedis(configuration);
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
    
    private static void AddDbRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var rr = configuration.GetConnectionString("RedisConnection")!;
       services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!)
        );

        services.AddScoped(sp =>
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase()
        );
        
        services.AddScoped<IQueueService, QueueService>();
    }
}