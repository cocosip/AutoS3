using Amazon.S3;
using AmazonKS3;
using Microsoft.Extensions.Logging;
using System;

namespace AutoS3.KS3
{
    /// <summary>
    /// KS3 client builder
    /// </summary>
    public class KS3ClientBuilder : IS3ClientBuilder
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        public KS3ClientBuilder(ILogger<KS3ClientBuilder> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// S3Vendor
        /// </summary>
        public S3VendorType S3Vendor => S3VendorType.KS3;


        /// <summary>
        /// Build a amazon s3 client <see cref="Amazon.S3.IAmazonS3"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IAmazonS3 BuildClient(S3ClientConfiguration configuration)
        {
            _logger.LogDebug("Create client with configuration:{0}", configuration.ToString());

            if (configuration.Config is AmazonKS3Config kS3Config)
            {
                IAmazonS3 client = new AmazonKS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, kS3Config);
                return client;
            }

            throw new ArgumentException($"The configuration {configuration}, create ks3 client fail, config was not a type of 'AmazonKS3Config'! ");

        }


    }
}
