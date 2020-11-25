using Amazon.S3;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace AutoS3.Tests
{
    public class DefaultS3ClientFactoryTest
    {


        [Fact]
        public void Get_Configuration_Empty_Test()
        {
            var mockIAmazonS3 = new Mock<IAmazonS3>();
            var mockLogger = new Mock<ILogger<DefaultS3ClientFactory>>();
            var mockS3ClientConfigurationSelector = new Mock<IS3ClientConfigurationSelector>();
            var mockS3ClientPoolFactory = new Mock<IS3ClientPoolFactory>();

            IS3ClientFactory s3ClientFactory = new DefaultS3ClientFactory(mockLogger.Object, mockS3ClientConfigurationSelector.Object, mockS3ClientPoolFactory.Object);

            Assert.Throws<ArgumentNullException>(() =>
            {
                s3ClientFactory.Get("t1");
            });
        }


        [Fact]
        public void Get_Test()
        {
            var mockIAmazonS3 = new Mock<IAmazonS3>();
            var mockLogger = new Mock<ILogger<DefaultS3ClientFactory>>();
            var s3ClientPool = new Mock<IS3ClientPool>();
            s3ClientPool.Setup(x => x.Get())
                .Returns(mockIAmazonS3.Object);

            var mockS3ClientConfigurationSelector = new Mock<IS3ClientConfigurationSelector>();
            var mockS3ClientPoolFactory = new Mock<IS3ClientPoolFactory>();

            mockS3ClientPoolFactory
                .Setup(x => x.Create(It.IsAny<S3ClientConfiguration>()))
                .Returns(s3ClientPool.Object);

            IS3ClientFactory s3ClientFactory = new DefaultS3ClientFactory(mockLogger.Object, mockS3ClientConfigurationSelector.Object, mockS3ClientPoolFactory.Object);

            var s3Client1 = s3ClientFactory.GetOrAdd("123456", "123456", () =>
            {
                return new S3ClientConfiguration()
                {
                    Vendor = S3VendorType.Amazon,
                    AccessKeyId = "123456",
                    SecretAccessKey = "123456",
                    MaxClient = 3
                };
            });
            var s3Client2 = s3ClientFactory.GetWithAccessSecret("123456", "123456");

            mockS3ClientPoolFactory.Verify(x => x.Create(It.IsAny<S3ClientConfiguration>()), Times.Once);

            var s3Client3 = s3ClientFactory.GetOrAdd("111", "222", () =>
            {
                return new S3ClientConfiguration()
                {
                    Vendor = S3VendorType.KS3,
                    AccessKeyId = "111",
                    SecretAccessKey = "222",
                    MaxClient = 2
                };
            });
            var s3Client4 = s3ClientFactory.GetWithAccessSecret("111", "222");
            mockS3ClientPoolFactory.Verify(x => x.Create(It.IsAny<S3ClientConfiguration>()), Times.Between(1, 3, Moq.Range.Exclusive));

            Assert.Throws<ArgumentNullException>(() =>
            {
                s3ClientFactory.GetWithAccessSecret("123456", "222");
            });

        }



    }
}
