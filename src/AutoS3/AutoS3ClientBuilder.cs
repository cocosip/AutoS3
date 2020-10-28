using Amazon.S3;
using Microsoft.Extensions.Logging;

namespace AutoS3
{
    public class AutoS3ClientBuilder : IS3ClientBuilder
    {
        private readonly ILogger _logger;
        public AutoS3ClientBuilder(ILogger<AutoS3ClientBuilder> logger)
        {
            _logger = logger;
        }

        public S3VendorType S3Vendor => S3VendorType.Amazon;

        /// <summary>
        /// Build a amazon s3 client <see cref="Amazon.S3.IAmazonS3"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IAmazonS3 BuildClient(S3ClientConfiguration configuration)
        {
            _logger.LogDebug("Create client with configuration:{0}", configuration.ToString());
            IAmazonS3 client = new AmazonS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, configuration.Config);
            return client;
        }
    }
}
