using Ambev.DeveloperEvaluation.IoC.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.Common.Cache;

public static class CacheExtension
{
    public static IServiceCollection AddRedisCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnectionName = "RedisConnection";

        var redisConnection = configuration.GetConnectionString(redisConnectionName);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString(redisConnectionName)!)
        );

        object value = services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString(redisConnectionName);
            options.InstanceName = CacheConstants.SaleInstancePrefix;
        });

        services.AddSingleton<ICacheManager, RedisCacheManager>();

        return services;
    }
}
