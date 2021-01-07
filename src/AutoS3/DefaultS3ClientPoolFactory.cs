using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoS3
{
    /// <summary>
    /// S3 client pool factory
    /// </summary>
    public class DefaultS3ClientPoolFactory : IS3ClientPoolFactory
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IS3ClientBuilder> _s3ClientBuilders;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="s3ClientBuilders"></param>
        public DefaultS3ClientPoolFactory(
            ILogger<DefaultS3ClientPoolFactory> logger,
            IServiceProvider serviceProvider, 
            IEnumerable<IS3ClientBuilder> s3ClientBuilders)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _s3ClientBuilders = s3ClientBuilders;
        }

        /// <summary>
        /// Create s3 client pool by configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IS3ClientPool Create(S3ClientConfiguration configuration)
        {
            var logger = _serviceProvider.GetService<ILogger<DefaultS3ClientPool>>();

            var s3ClientBuilder = FindS3ClientBuilder(configuration.Vendor);
            if (s3ClientBuilder == null)
            {
                throw new ArgumentNullException($"Could not find any 'IS3ClientBuilder' by vendor '{configuration.Vendor}'");
            }

            IS3ClientPool s3ClientPool = new DefaultS3ClientPool(logger, configuration, s3ClientBuilder);
            _logger.LogDebug("Create new S3ClientPool with configuration:{0}.", configuration);
            return s3ClientPool;
        }


        private IS3ClientBuilder FindS3ClientBuilder(S3VendorType s3Vendor)
        {
            return _s3ClientBuilders.FirstOrDefault(x => x.S3Vendor == s3Vendor);
        }

    }
}
