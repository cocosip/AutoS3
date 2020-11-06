namespace AutoS3.Sample
{
    public class SampleAppOptions
    {
        public S3VendorType S3Vendor { get; set; }

        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public string ServerUrl { get; set; }

        public bool ForcePathStyle { get; set; } = true;

        public string SignatureVersion { get; set; }

        public string DefaultBucket { get; set; }

        public string SimpleUploadFilePath { get; set; }

        public string MultipartUploadFilePath { get; set; }
    }
}
