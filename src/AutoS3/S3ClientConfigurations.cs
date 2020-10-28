using System;
using System.Collections.Generic;

namespace AutoS3
{
    public class S3ClientConfigurations
    {
        private S3ClientConfiguration Default => GetConfiguration<DefaultS3Client>();

        private readonly Dictionary<string, S3ClientConfiguration> _clients;

        public S3ClientConfigurations()
        {
            _clients = new Dictionary<string, S3ClientConfiguration>
            {
                //Add default client
                [S3ClientNameAttribute.GetClientName<DefaultS3Client>()] = new S3ClientConfiguration()
                {
                    Vendor = S3VendorType.Amazon
                }
            };
        }

        public S3ClientConfigurations Configure<TClient>(
            Action<S3ClientConfiguration> configureAction)
        {
            return Configure(
                S3ClientNameAttribute.GetClientName<TClient>(),
                configureAction
            );
        }

        public S3ClientConfigurations Configure(
            string name,
            Action<S3ClientConfiguration> configureAction)
        {
            if (!_clients.TryGetValue(name, out S3ClientConfiguration s3Client))
            {
                s3Client = new S3ClientConfiguration()
                {
                    Vendor = Default.Vendor
                };

                _clients.Add(name, s3Client);
            }

            configureAction(s3Client);

            return this;
        }

        public S3ClientConfigurations ConfigureDefault(Action<S3ClientConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public S3ClientConfigurations ConfigureAll(Action<string, S3ClientConfiguration> configureAction)
        {
            foreach (var client in _clients)
            {
                configureAction(client.Key, client.Value);
            }

            return this;
        }

        public S3ClientConfiguration GetConfiguration<TClient>()
        {
            return GetConfiguration(S3ClientNameAttribute.GetClientName<TClient>());
        }

        public S3ClientConfiguration GetConfiguration(string name)
        {

            _clients.TryGetValue(name, out S3ClientConfiguration s3Client);
            return s3Client ?? Default;
        }

    }
}
