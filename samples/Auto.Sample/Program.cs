using AutoS3.KS3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutoS3.Sample
{
    class Program
    {
        private static SampleAppService _sampleAppService;

        static void Main(string[] args)
        {
            Console.WriteLine("使用AWSSDK.S3 SDK调用金山云KS3");

            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("da9eaeec-3e05-422b-a1f8-31a01a820cb0");


            var configuration = builder.Build();
            var services = new ServiceCollection();
            services
                .AddLogging(l => l.AddConsole())
                .AddAutoS3()
                .AddAutoKS3()
                .Configure<SampleAppOptions>(configuration.GetSection("SampleAppOptions"))
                .AddSingleton<SampleAppService>();

            var provider = services.BuildServiceProvider();

            _sampleAppService = provider.GetService<SampleAppService>();
            Run();
            Console.ReadLine();
        }


        public static async void Run()
        {
            //列出Bucket
            await _sampleAppService.ListBucketsAsync();

            //列出对象
            await _sampleAppService.ListObjectsAsync();

            // 获取Bucket权限
            await _sampleAppService.GetAclAsync();

            //简单上传
            var simpleUploadKey = await _sampleAppService.SimpleUploadAsync();

            //下载文件
            await _sampleAppService.SimpleGetObjectAsync(simpleUploadKey);

            //获取预授权地址
            var url1 = _sampleAppService.GetPreSignedURL(simpleUploadKey);
            //生成预授权地址
            var url2 = _sampleAppService.GeneratePreSignedURL(simpleUploadKey);

            //下载Copy
            await _sampleAppService.SimpleGetObjectAsync(simpleUploadKey);

            //拷贝文件key
            var copyKey = await _sampleAppService.CopyObjectAsync(simpleUploadKey);

            //删除文件
            await _sampleAppService.DeleteObject(simpleUploadKey);
            await _sampleAppService.DeleteObject(copyKey);
            ////分片上传
            //var multipartUploadKey = await MultipartUpload();

            ////Url
            //var multipartUrl = GeneratePreSignedURL(multipartUploadKey);

            ////获取文件信息
            //await GetMetadata(multipartUploadKey);

            ////获取ACL
            //await GetACL(multipartUploadKey);

        }

    }
}
