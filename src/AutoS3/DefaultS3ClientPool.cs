using Amazon.S3;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AutoS3
{
    /// <summary>
    /// S3 client pool
    /// </summary>
    public class DefaultS3ClientPool : IS3ClientPool
    {
        private readonly ILogger _logger;
        private readonly S3ClientConfiguration _configuration;
        private readonly IS3ClientBuilder _s3ClientBuilder;

        private int _sequence = 1;
        private readonly object _sync = new object();
        private readonly ConcurrentDictionary<int, IAmazonS3> _clients;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="s3ClientBuilder"></param>
        public DefaultS3ClientPool(
            ILogger<DefaultS3ClientPool> logger,
            S3ClientConfiguration configuration, 
            IS3ClientBuilder s3ClientBuilder)
        {
            _logger = logger;

            _configuration = configuration;
            _s3ClientBuilder = s3ClientBuilder;

            _clients = new ConcurrentDictionary<int, IAmazonS3>();
        }

        /// <summary>
        /// Get a amazon s3 client,if client not exist, create a new client
        /// </summary>
        /// <returns></returns>
        public IAmazonS3 Get()
        {
            var index = _sequence % _configuration.MaxClient;
            if (!_clients.TryGetValue(index, out IAmazonS3 client))
            {
                lock (_sync)
                {
                    if (!_clients.TryGetValue(index, out client))
                    {
                        client = _s3ClientBuilder.BuildClient(_configuration);
                        if (!_clients.TryAdd(index, client))
                        {
                            _logger.LogWarning("Add client to dict fail with configuration:{0}.", _configuration.ToString());
                        }
                    }
                }
            }

            if (client == null)
            {
                throw new ArgumentNullException("Could not get any amazon s3 client!");
            }

            Interlocked.Increment(ref _sequence);
            return client;
        }
    }
}
