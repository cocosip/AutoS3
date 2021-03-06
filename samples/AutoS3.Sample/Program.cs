﻿using AutoS3.KS3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AutoS3.Sample
{
    class Program
    {
        private static SampleAppService _sampleAppService;

        static void Main(string[] args)
        {
            Console.WriteLine("使用AWSSDK.S3 SDK 调用存储测试");

            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("da9eaeec-3e05-422b-a1f8-31a01a820cb0");


            var configuration = builder.Build();
            var services = new ServiceCollection();
            services
                .AddLogging(l =>
                {
                    l.AddConsole();
                    l.SetMinimumLevel(LogLevel.Debug);
                })
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
            _ = _sampleAppService.GetPreSignedURL(simpleUploadKey);
            //生成预授权地址
            _ = _sampleAppService.GeneratePreSignedURL(simpleUploadKey);

            //下载Copy
            await _sampleAppService.SimpleGetObjectAsync(simpleUploadKey);

            //拷贝文件key
            var copyKey = await _sampleAppService.CopyObjectAsync(simpleUploadKey);

            //删除文件
            await _sampleAppService.DeleteObject(simpleUploadKey);
            await _sampleAppService.DeleteObject(copyKey);

            //分片上传
            var multipartUploadKey = await _sampleAppService.MultipartUploadAsync();

            //Url
            _ = _sampleAppService.GeneratePreSignedURL(multipartUploadKey);

            //删除文件
            await _sampleAppService.DeleteObject(multipartUploadKey);


        }

    }
}
