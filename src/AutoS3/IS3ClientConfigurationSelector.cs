namespace AutoS3
{
    /// <summary>
    /// S3 client configuration selector
    /// </summary>
    public interface IS3ClientConfigurationSelector
    {
        /// <summary>
        /// Get s3 client configuration by name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        S3ClientConfiguration Get(string name);
    }
}
