using AutoS3.Tests.TestObjects;
using Xunit;

namespace AutoS3.Tests
{
    public class S3ClientNameAttributeTest
    {
        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name = S3ClientNameAttribute
                  .GetClientName<TestClient2>();

            Assert.Equal("client2", name);
        }

        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestClient1).FullName;

            var name = S3ClientNameAttribute
                  .GetClientName<TestClient1>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var expected = typeof(TestClient3).FullName;
            var name = S3ClientNameAttribute.GetClientName(typeof(TestClient3));
            Assert.Equal(expected, name);
        }
    }
}
