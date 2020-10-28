using Amazon.S3;

namespace AutoS3
{
    public interface IS3ClientBuilder
    {
        public S3VendorType S3Vendor { get; }

        /// <summary>
        /// Build a amazon s3 client <see cref="Amazon.S3.IAmazonS3"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IAmazonS3 BuildClient(S3ClientConfiguration configuration);
    }
}
