using Amazon.S3;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AutoS3.Tests
{
    public class DefaultS3ClientPoolTest
    {

        [Fact]
        public void Get_BuildClient_Times_Test()
        {
            var s3ClientConfiguration = new S3ClientConfiguration()
            {
                Vendor = S3VendorType.Amazon,
                AccessKeyId = "123456",
                SecretAccessKey = "12345",
                MaxClient = 2,
                Config = new AmazonS3Config()
                {
                    ServiceURL = "http://192.168.0.100",
                    ForcePathStyle = true,
                    SignatureVersion = "2.0"
                }
            };
            var mockLogger = new Mock<ILogger<DefaultS3ClientPool>>();
            var mockIAmazonS3 = new Mock<IAmazonS3>();

            var mockS3ClientBuilder = new Mock<IS3ClientBuilder>();
            mockS3ClientBuilder.Setup(x => x.BuildClient(It.IsAny<S3ClientConfiguration>())).Returns(mockIAmazonS3.Object);

            IS3ClientPool s3ClientPool = new DefaultS3ClientPool(mockLogger.Object, s3ClientConfiguration, mockS3ClientBuilder.Object);

            var client1 = s3ClientPool.Get();
            var client2 = s3ClientPool.Get();
            var client3 = s3ClientPool.Get();

            mockS3ClientBuilder.Verify(x => x.BuildClient(It.IsAny<S3ClientConfiguration>()), Times.Between(1, 3, Range.Exclusive));
        }

    }
}
