using Microsoft.Extensions.DependencyInjection;

namespace AutoS3.KS3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoKS3(this IServiceCollection services)
        {
            services.AddTransient<IS3ClientBuilder, KS3ClientBuilder>();

            return services;
        }
    }
}
