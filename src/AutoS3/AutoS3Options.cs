namespace AutoS3
{
    /// <summary>
    /// AutoS3 options
    /// </summary>
    public class AutoS3Options
    {
        /// <summary>
        /// S3 client configurations
        /// </summary>
        public S3ClientConfigurations Clients { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public AutoS3Options()
        {
            Clients = new S3ClientConfigurations();
        }
    }
}
