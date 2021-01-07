using Amazon.S3;

namespace AutoS3
{
    /// <summary>
    /// S3 Client Configuration
    /// </summary>
    public class S3ClientConfiguration
    {
        /// <summary>
        /// S3存储类型
        /// </summary>
        public S3VendorType Vendor { get; set; }

        /// <summary>
        /// AccessKeyId
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// SecretAccessKey
        /// </summary>
        public string SecretAccessKey { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public AmazonS3Config Config { get; set; }

        /// <summary>
        /// 最大客户端数量
        /// </summary>
        public int MaxClient { get; set; } = 20;

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"S3ClientConfiguration:[Vendor:{Vendor},AccessKeyId:{AccessKeyId},SecretAccessKey:{SecretAccessKey},MaxClient:{MaxClient}]";
        }

    }
}
