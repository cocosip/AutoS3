namespace AutoS3
{
    public class AutoS3Options
    {
        public S3ClientConfigurations Clients { get; }

        public AutoS3Options()
        {
            Clients = new S3ClientConfigurations();
        }
    }
}
