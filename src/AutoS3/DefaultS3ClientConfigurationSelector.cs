using Microsoft.Extensions.Options;

namespace AutoS3
{

    /// <summary>
    /// S3Client Configuration Selector 
    /// </summary>
    public class DefaultS3ClientConfigurationSelector : IS3ClientConfigurationSelector
    {
        private readonly AutoS3Options _options;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options"></param>
        public DefaultS3ClientConfigurationSelector(IOptions<AutoS3Options> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Get s3 client configuration by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public S3ClientConfiguration Get(string name)
        {
            return _options.Clients.GetConfiguration(name);
        }

    }
}
