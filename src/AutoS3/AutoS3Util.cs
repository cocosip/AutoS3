using System.Security.Cryptography;
using System.Text;

namespace AutoS3
{
    /// <summary>
    /// AutoS3 util class
    /// </summary>
    public static class AutoS3Util
    {

        /// <summary>
        /// Calculate client name by ak,sk
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="secretAccessKey"></param>
        /// <returns></returns>
        public static string CalculateClientName(string accessKeyId, string secretAccessKey)
        {
            var source = $"{accessKeyId}{secretAccessKey}";
            using var sha1 = SHA1.Create();
            var buffer = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
