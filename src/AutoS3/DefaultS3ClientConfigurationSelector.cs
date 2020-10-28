using Microsoft.Extensions.Options;

namespace AutoS3
{
    public class DefaultS3ClientConfigurationSelector : IS3ClientConfigurationSelector
    {
        private readonly AutoS3Options _options;
        public DefaultS3ClientConfigurationSelector(IOptions<AutoS3Options> options)
        {
            _options = options.Value;
        }

        public S3ClientConfiguration Get(string name)
        {
            return _options.Clients.GetConfiguration(name);
        }

    }
}
