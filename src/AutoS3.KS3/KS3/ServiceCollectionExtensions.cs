using Microsoft.Extensions.DependencyInjection;

namespace AutoS3.KS3
{
    /// <summary>
    /// DependencyInjection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add AutoS3 KS3
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoKS3(this IServiceCollection services)
        {
            services.AddTransient<IS3ClientBuilder, KS3ClientBuilder>();
            return services;
        }
    }
}
