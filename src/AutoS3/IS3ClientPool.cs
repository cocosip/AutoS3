﻿using Amazon.S3;

namespace AutoS3
{
    public interface IS3ClientPool
    {
        /// <summary>
        /// Get a amazon s3 client,if client not exist, create a new client
        /// </summary>
        /// <returns></returns>
        IAmazonS3 Get();
    }
}
