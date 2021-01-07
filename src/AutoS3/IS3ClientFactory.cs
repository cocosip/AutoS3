using Amazon.S3;
using System;

namespace AutoS3
{
    /// <summary>
    /// S3 client factory
    /// </summary>
    public interface IS3ClientFactory
    {
        /// <summary>
        /// Get IAmazonS3 with client name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAmazonS3 Get(string name);

        /// <summary>
        /// Get IAmazonS3 with accessKeyId and secretAccessKey
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <returns></returns>
        IAmazonS3 GetWithAccessSecret(string accessKeyId, string secretAccessKey);

        /// <summary>
        /// Get IAmazonS3 with accessKeyId and secretAccessKey, if not exist create a new client with configuration action
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAmazonS3 GetOrAdd(string accessKeyId, string secretAccessKey, Func<S3ClientConfiguration> factory);

        /// <summary>
        /// Get IAmazonS3 with name, if not exist create a new client with configuration action
        /// </summary>
        /// <param name="name"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAmazonS3 GetOrAdd(string name, Func<S3ClientConfiguration> factory);

        /// <summary>
        /// Whether there are any s3 client pool with accessKeyId,secretAccessKey was in the dict
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <returns></returns>
        bool HasAccessSecret(string accessKeyId, string secretAccessKey);
    }
}
