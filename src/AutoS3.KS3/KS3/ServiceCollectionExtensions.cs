using Microsoft.Extensions.DependencyInjection;

namespace AutoS3.KS3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKS3ClientBuilder(this IServiceCollection services)
        {
            services.AddTransient<IS3ClientBuilder, KS3ClientBuilder>();

            return services;
        }
    }
}
