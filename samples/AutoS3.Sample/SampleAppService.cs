using Amazon.S3;
using Amazon.S3.Model;
using AmazonKS3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoS3.Sample
{
    public class SampleAppService
    {
        private readonly ILogger _logger;
        private readonly SampleAppOptions _options;
        private readonly IS3ClientFactory _s3ClientFactory;
        public SampleAppService(ILogger<SampleAppService> logger, IOptions<SampleAppOptions> options, IS3ClientFactory s3ClientFactory)
        {
            _logger = logger;
            _options = options.Value;
            _s3ClientFactory = s3ClientFactory;
        }


        protected IAmazonS3 GetClient()
        {
            return _s3ClientFactory.GetOrAdd("default", () =>
            {
                var configuration = new S3ClientConfiguration()
                {
                    Vendor = _options.S3Vendor,
                    AccessKeyId = _options.AccessKeyId,
                    SecretAccessKey = _options.SecretAccessKey,
                    MaxClient = 10
                };

                if (_options.S3Vendor == S3VendorType.Amazon)
                {
                    configuration.Config = new AmazonS3Config()
                    {
                        ServiceURL = _options.ServerUrl,
                        ForcePathStyle = _options.ForcePathStyle,
                        SignatureVersion = _options.SignatureVersion
                    };
                }
                else
                {
                    configuration.Config = new AmazonKS3Config()
                    {
                        ServiceURL = _options.ServerUrl,
                        ForcePathStyle = _options.ForcePathStyle,
                        SignatureVersion = _options.SignatureVersion
                    };
                }
                return configuration;
            });
        }


        /// <summary>列出Bucket
        /// </summary>
        public async Task ListBucketsAsync()
        {
            var listBucketsResponse = await GetClient().ListBucketsAsync();
            _logger.LogInformation("---列出Buckets---,OwnerId:{0}", listBucketsResponse.Owner.Id);
            foreach (var bucket in listBucketsResponse.Buckets)
            {
                _logger.LogInformation("BucketName:{0},创建时间:{1}", bucket.BucketName, bucket.CreationDate.ToString("yyyy-MM-dd HH:mm"));
            }
        }

        /// <summary>获取Bucket权限
        /// </summary>
        public async Task GetAclAsync()
        {
            _logger.LogInformation("---获取当前Bucket的权限---");

            var getACLResponse = await GetClient().GetACLAsync(new GetACLRequest()
            {
                BucketName = _options.DefaultBucket
            });

            foreach (var grant in getACLResponse.AccessControlList.Grants)
            {
                _logger.LogInformation("当前Bucket权限:{0}", grant.Permission.Value);
            }
        }


        /// <summary>列出对象
        /// </summary>
        public async Task ListObjectsV2Async(string prefix = "", string delimiter = "", int count = 10)
        {
            //查询文件
            _logger.LogInformation("---列出Bucket,'Prefix:{0}','Delimiter:{1}','Count:{2}'---", prefix, delimiter, count);

            var listObjectsV2Response = await GetClient().ListObjectsV2Async(new ListObjectsV2Request()
            {
                BucketName = _options.DefaultBucket,
                Prefix = prefix,
                Delimiter = delimiter,
                MaxKeys = count
            });
            foreach (var s3Object in listObjectsV2Response.S3Objects)
            {
                _logger.LogInformation("S3对象Key:{0}", s3Object.Key);
            }
        }


        /// <summary>列出对象
        /// </summary>
        public async Task ListObjectsAsync(string prefix = "", string delimiter = "", int count = 10)
        {
            //查询文件
            _logger.LogInformation("---列出Bucket前10个文件---");

            var listObjectsResponse = await GetClient().ListObjectsAsync(new ListObjectsRequest()
            {
                BucketName = _options.DefaultBucket,
                //Prefix = prefix,
                //Delimiter = delimiter,
                MaxKeys = count
            });

            foreach (var s3Object in listObjectsResponse.S3Objects)
            {
                _logger.LogInformation("S3对象Key:{0}", s3Object.Key);
            }
        }



        /// <summary>获取指定的文件
        /// </summary>
        private async Task GetObjectAsync(string key)
        {
            //查询文件
            _logger.LogInformation("---获取指定的文件---");

            var getObjectResponse = await GetClient().GetObjectAsync(new GetObjectRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key,
            });

            _logger.LogInformation("获取对象返回:对象Key:{0},ETag:{1}", getObjectResponse?.Key, getObjectResponse?.ETag);
        }

        /// <summary>简单上传文件
        /// </summary>
        public async Task<string> SimpleUploadAsync()
        {

            var ext = ".txt";

            if (_options.UseLocalFile)
            {
                ext = _options.SimpleUploadFilePath.Substring(_options.SimpleUploadFilePath.LastIndexOf('.'));
            }

            var key = $"test/{Guid.NewGuid()}{ext}";
            _logger.LogInformation("---上传简单文件,上传Key:{0}---", key);

            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = _options.DefaultBucket,
                AutoCloseStream = true,
                Key = key,
                UseChunkEncoding = _options.UseChunkEncoding,
            };

            //使用本地测试文件
            if (_options.UseLocalFile)
            {
                putObjectRequest.FilePath = _options.SimpleUploadFilePath;
            }
            else
            {

                var ms = new MemoryStream(Encoding.UTF8.GetBytes("Hello AutoS3!"));
                ms.Seek(0, SeekOrigin.Begin);

                //自动生成流
                putObjectRequest.InputStream = ms;
            }

            //进度条
            putObjectRequest.StreamTransferProgress += (sender, args) =>
            {
                _logger.LogInformation("ProgressCallback - Progress: {0}%, TotalBytes:{1}, TransferredBytes:{2} ",
                    args.TransferredBytes * 100 / args.TotalBytes, args.TotalBytes, args.TransferredBytes);
            };

            var putObjectResponse = await GetClient().PutObjectAsync(putObjectRequest);
            _logger.LogInformation("简单上传成功,Etag:{0}", putObjectResponse.ETag);
            return key;
        }

        /// <summary>简单下载文件
        /// </summary>
        public async Task SimpleGetObjectAsync(string key)
        {
            _logger.LogInformation("---简单下载文件,Key:{0}---", key);
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key
            };

            var getObjectResponse = await GetClient().GetObjectAsync(getObjectRequest);

            _logger.LogInformation("简单下载文件成功,Key:{0}", getObjectResponse.Key);
        }


        /// <summary>获取预授权地址
        /// </summary>
        public string GetPreSignedURL(string key)
        {
            //获取文件下载地址
            _logger.LogInformation("---获取预授权地址---");

            var url = GetClient().GetPreSignedURL(new GetPreSignedUrlRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key,
                Expires = DateTime.Now.AddMinutes(5),
            });
            _logger.LogInformation("获取预授权地址:{0}", url);
            return url;
        }

        /// <summary>生成预授权地址
        /// </summary>
        public string GeneratePreSignedURL(string key)
        {
            _logger.LogInformation("---生成预授权地址---");

            var url = GetClient().GeneratePreSignedURL(_options.DefaultBucket, key, DateTime.Now.AddMinutes(10), null);
            _logger.LogInformation("生成预授权地址:{0}", url);
            return url;
        }


        /// <summary>拷贝文件
        /// </summary>
        public async Task<string> CopyObjectAsync(string key)
        {
            var ext = ".txt";
            if (!string.IsNullOrWhiteSpace(key))
            {
                ext = key.Substring(key.LastIndexOf('.'));
            }
            var destinationKey = $"copyfiles/{Guid.NewGuid()}{ext}";
            _logger.LogInformation("---拷贝文件,目标:{0}---", destinationKey);

            var copyObjectRequest = new CopyObjectRequest()
            {
                SourceBucket = _options.DefaultBucket,
                DestinationBucket = _options.DefaultBucket,
                SourceKey = key,
                DestinationKey = destinationKey,
            };

            var copyObjectResponse = await GetClient().CopyObjectAsync(copyObjectRequest);


            _logger.LogInformation("拷贝文件成功,RequestId:{0},拷贝目标Key:{1}", copyObjectResponse.ResponseMetadata.RequestId, destinationKey);

            return destinationKey;

        }

        /// <summary>删除文件
        /// </summary>
        public async Task DeleteObject(string key)
        {
            _logger.LogInformation("---删除文件---,Key:{0}", key);

            var deleteObjectResponse = await GetClient().DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key
            });

            _logger.LogInformation("删除文件成功,DeleteMarker:{0}", deleteObjectResponse.DeleteMarker);
        }


        /// <summary>分片上传
        /// </summary>
        public async Task<string> MultipartUploadAsync()
        {
            var key = $"test/{Guid.NewGuid()}.dcm";
            _logger.LogInformation("---分片上传文件,上传Key:{0}---", key);

            //初始化分片上传
            var initiateMultipartUploadResponse = await GetClient().InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key,
            });

            //上传Id
            var uploadId = initiateMultipartUploadResponse.UploadId;
            // 计算分片总数。
            var partSize = 5 * 1024 * 1024;
            var fi = new FileInfo(_options.MultipartUploadFilePath);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }
            // 开始分片上传。partETags是保存partETag的列表，OSS收到用户提交的分片列表后，会逐一验证每个分片数据的有效性。 当所有的数据分片通过验证后，OSS会将这些分片组合成一个完整的文件。
            var partETags = new List<PartETag>();

            var uploadPartTasks = new List<Task>();

            using (var fs = File.Open(_options.MultipartUploadFilePath, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    var skipBytes = (long)partSize * i;
                    // 定位到本次上传起始位置。
                    //fs.Seek(skipBytes, 0);
                    // 计算本次上传的片大小，最后一片为剩余的数据大小。
                    var size = (int)((partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes));

                    byte[] buffer = new byte[size];
                    fs.Read(buffer, 0, size);

                    uploadPartTasks.Add(Task.Run<UploadPartResponse>(() =>
                    {
                        return GetClient().UploadPartAsync(new UploadPartRequest()
                        {
                            BucketName = _options.DefaultBucket,
                            UploadId = uploadId,
                            Key = key,
                            InputStream = new MemoryStream(buffer),
                            PartSize = size,
                            PartNumber = i + 1
                        });

                    }).ContinueWith(t =>
                    {
                        partETags.Add(new PartETag(t.Result.PartNumber, t.Result.ETag));
                        _logger.LogInformation("finish {0}/{1}", partETags.Count, partCount);
                    }));

                    //分片上传
                    //var uploadPartResponse = await Client.UploadPartAsync(new UploadPartRequest()
                    //{
                    //    BucketName = BucketName,
                    //    UploadId = uploadId,
                    //    Key = key,
                    //    InputStream = new MemoryStream(buffer),
                    //    PartSize = size,
                    //    PartNumber = i + 1
                    //});

                    //partETags.Add(new PartETag(uploadPartResponse.PartNumber, uploadPartResponse.ETag));
                    //Console.WriteLine("finish {0}/{1}", partETags.Count, partCount);
                }
                //Console.WriteLine("分片上传完成");
            }

            Task.WaitAll(uploadPartTasks.ToArray());

            _logger.LogInformation("共:{0}个PartETags", partETags.Count);

            //列出所有分片
            _logger.LogInformation("---列出所有分片,UploadId:{0}---", uploadId);

            var listPartsResponse = await GetClient().ListPartsAsync(new ListPartsRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key,
                UploadId = uploadId
            });
            foreach (var part in listPartsResponse.Parts)
            {
                _logger.LogInformation("分片序号:{0},分片ETag:{1}", part.PartNumber, part.ETag);
            }


            var completeMultipartUploadResponse = await GetClient().CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest()
            {
                BucketName = _options.DefaultBucket,
                Key = key,
                UploadId = uploadId,
                PartETags = partETags
            });

            _logger.LogInformation("分片上传完成,Key:{0}", completeMultipartUploadResponse.Key);
            return completeMultipartUploadResponse.Key;
        }

    }
}
