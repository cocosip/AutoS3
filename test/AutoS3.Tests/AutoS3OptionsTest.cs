using AutoS3;
using AutoS3.Tests.TestObjects;
using Xunit;

namespace AmazonS3.Tests
{
    public class AutoS3OptionsTest
    {
        [Fact]
        public void Get_Set_AmazonS3Options_Test()
        {
            var options = new AutoS3Options();
            options.Clients.ConfigureDefault(c =>
            {
                c.Vendor = S3VendorType.Amazon;
                c.AccessKeyId = "123456";
                c.SecretAccessKey = "654321";
                c.MaxClient = 1;
            });

            options.Clients.Configure<TestClient2>(c =>
            {
                c.Vendor = S3VendorType.KS3;
                c.AccessKeyId = "222";
                c.SecretAccessKey = "222222";
                c.MaxClient = 66;
            });

            options.Clients.Configure("test3", c =>
            {
                c.Vendor = S3VendorType.Amazon;
                c.AccessKeyId = "333333";
                c.SecretAccessKey = "111111";
                c.MaxClient = 3;
            });

            options.Clients.Configure("test4", c =>
            {
                c.Vendor = S3VendorType.KS3;
                c.AccessKeyId = "444";
                c.SecretAccessKey = "4444";
                c.MaxClient = 44;
            });


            options.Clients.ConfigureAll((n, c) =>
            {
                if (n == "test4")
                {
                    c.Vendor = S3VendorType.Amazon;
                    c.AccessKeyId = "10";
                    c.SecretAccessKey = "20";
                }
            });



            var configuration1 = options.Clients.GetConfiguration(DefaultS3Client.Name);
            Assert.Equal(S3VendorType.Amazon, configuration1.Vendor);
            Assert.Equal("123456", configuration1.AccessKeyId);
            Assert.Equal("654321", configuration1.SecretAccessKey);
            Assert.Equal(1, configuration1.MaxClient);

            var configuration2 = options.Clients.GetConfiguration<TestClient2>();
            Assert.Equal(S3VendorType.KS3, configuration2.Vendor);
            Assert.Equal("222", configuration2.AccessKeyId);
            Assert.Equal("222222", configuration2.SecretAccessKey);
            Assert.Equal(66, configuration2.MaxClient);

            var configuration3 = options.Clients.GetConfiguration("test3");
            Assert.Equal(S3VendorType.Amazon, configuration3.Vendor);
            Assert.Equal("333333", configuration3.AccessKeyId);
            Assert.Equal("111111", configuration3.SecretAccessKey);
            Assert.Equal(3, configuration3.MaxClient);

            var configuration4 = options.Clients.GetConfiguration("test4");
            Assert.Equal(S3VendorType.Amazon, configuration4.Vendor);
            Assert.Equal("10", configuration4.AccessKeyId);
            Assert.Equal("20", configuration4.SecretAccessKey);
            Assert.Equal(44, configuration4.MaxClient);

        }

    }
}
