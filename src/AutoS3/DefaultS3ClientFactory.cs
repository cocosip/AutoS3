using Amazon.S3;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace AutoS3
{
    public class DefaultS3ClientFactory : IS3ClientFactory
    {
        private readonly ILogger _logger;
        private readonly IS3ClientConfigurationSelector _configurationSelector;
        private readonly IS3ClientPoolFactory _s3ClientPoolFactory;

        private readonly object _sync = new object();
        private readonly ConcurrentDictionary<string, IS3ClientPool> _s3ClientPools;

        public DefaultS3ClientFactory(ILogger<DefaultS3ClientFactory> logger, IS3ClientConfigurationSelector configurationSelector, IS3ClientPoolFactory s3ClientPoolFactory)
        {
            _logger = logger;
            _configurationSelector = configurationSelector;
            _s3ClientPoolFactory = s3ClientPoolFactory;
            _s3ClientPools = new ConcurrentDictionary<string, IS3ClientPool>();
        }

        /// <summary>
        /// Get IAmazonS3 with client name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IAmazonS3 Get(string name)
        {
            var configuration = _configurationSelector.Get(name);
            if (configuration == null)
            {
                throw new ArgumentNullException($"Could not find any configuration by name '{name}'.");
            }

            var s3Client = GetClientInternal(name, configuration, true);

            if (s3Client == null)
            {
                throw new ArgumentNullException($"Could not find any s3 client by name '{name}',check your configurations!");
            }
            return s3Client;
        }

        /// <summary>
        /// Get IAmazonS3 with accessKeyId and secretAccessKey
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <returns></returns>
        public IAmazonS3 GetWithAccessSecret(string accessKeyId, string secretAccessKey)
        {
            var name = AutoS3Util.CalculateClientName(accessKeyId, secretAccessKey);
            var s3Client = GetClientInternal(name, createIfNotExist: false);
            if (s3Client == null)
            {
                throw new ArgumentNullException($"Could not find any s3 client with  AccessKeyId '{accessKeyId}', SecretAccessKey:{secretAccessKey}!");
            }
            return s3Client;
        }

        /// <summary>
        /// Get IAmazonS3 with accessKeyId and secretAccessKey, if not exist create a new client with configuration action
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IAmazonS3 GetOrAddClient(string accessKeyId, string secretAccessKey, Func<S3ClientConfiguration> factory)
        {
            var name = AutoS3Util.CalculateClientName(accessKeyId, secretAccessKey);
            var s3Client = GetClientInternal(name, factory(), true);
            if (s3Client == null)
            {
                throw new ArgumentNullException($"Could not find any s3 client with  AccessKeyId '{accessKeyId}', SecretAccessKey:{secretAccessKey},and also could not create new client!");
            }
            return s3Client;
        }


        /// <summary>
        /// Whether there are any s3 client pool with accessKeyId,secretAccessKey was in the dict
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <returns></returns>
        public bool HasAccessSecret(string accessKeyId, string secretAccessKey)
        {
            var name = AutoS3Util.CalculateClientName(accessKeyId, secretAccessKey);
            return _s3ClientPools.ContainsKey(name);
        }


        private IAmazonS3 GetClientInternal(string name, S3ClientConfiguration configuration = default, bool createIfNotExist = false)
        {
            if (!_s3ClientPools.TryGetValue(name, out IS3ClientPool s3ClientPool))
            {
                //Can't find any s3 client pool and do not create new
                if (!createIfNotExist)
                {
                    return null;
                }

                lock (_sync)
                {
                    if (!_s3ClientPools.TryGetValue(name, out s3ClientPool))
                    {
                        s3ClientPool = _s3ClientPoolFactory.Create(configuration);

                        if (!_s3ClientPools.TryAdd(name, s3ClientPool))
                        {
                            _logger.LogWarning("Failed to add s3 client pool to dict with configuration:{0}.", configuration);
                        }
                    }
                }
            }

            if (s3ClientPool == null)
            {
                throw new ArgumentNullException($"Could not find any s3 client pool with configuration {configuration}");
            }

            return s3ClientPool.Get();
        }
    }
}
