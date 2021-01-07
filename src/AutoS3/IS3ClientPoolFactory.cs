namespace AutoS3
{
    /// <summary>
    /// S3 client pool factory
    /// </summary>
    public interface IS3ClientPoolFactory
    {
        /// <summary>
        /// Create S3 client pool
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IS3ClientPool Create(S3ClientConfiguration configuration);
    }
}
