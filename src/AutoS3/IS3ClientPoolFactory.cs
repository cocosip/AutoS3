namespace AutoS3
{
    public interface IS3ClientPoolFactory
    {
        IS3ClientPool Create(S3ClientConfiguration configuration);
    }
}
