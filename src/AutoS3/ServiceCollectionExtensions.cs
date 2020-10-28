using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoS3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoS3(this IServiceCollection services, Action<AutoS3Options> configure = null)
        {
            configure ??= new Action<AutoS3Options>(c => { });

            services
                .Configure(configure)
                .AddSingleton<IS3ClientFactory, DefaultS3ClientFactory>()
                .AddTransient<IS3ClientConfigurationSelector, DefaultS3ClientConfigurationSelector>()
                .AddTransient<IS3ClientPoolFactory, DefaultS3ClientPoolFactory>()
                .AddTransient<IS3ClientBuilder, AutoS3ClientBuilder>()
                ;

            return services;
        }
    }
}
