namespace AutoS3
{
    public interface IS3ClientConfigurationSelector
    {
        S3ClientConfiguration Get(string name);
    }
}
