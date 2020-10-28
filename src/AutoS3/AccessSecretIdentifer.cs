using System;

namespace AutoS3
{
    public class AccessSecretIdentifer : IEquatable<AccessSecretIdentifer>
    {
        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public AccessSecretIdentifer()
        {

        }

        public AccessSecretIdentifer(string accessKeyId, string secretAccessKey)
        {
            AccessKeyId = accessKeyId;
            SecretAccessKey = secretAccessKey;
        }

        public bool Equals(AccessSecretIdentifer other)
        {
            return AccessKeyId == other.AccessKeyId && SecretAccessKey == other.SecretAccessKey;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is AccessSecretIdentifer other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(AccessKeyId) | StringComparer.InvariantCulture.GetHashCode(SecretAccessKey);
        }

        public static bool operator ==(AccessSecretIdentifer s1, AccessSecretIdentifer s2) => s1.Equals(s2);

        public static bool operator !=(AccessSecretIdentifer s1, AccessSecretIdentifer s2) => !s1.Equals(s2);
    }
}
