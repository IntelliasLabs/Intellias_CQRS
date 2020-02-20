using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    internal static class Extensions
    {
        public static string Zip(this string base64text)
        {
            var bytes = Encoding.UTF8.GetBytes(base64text);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string Unzip(this string base64text)
        {
            using (var msi = new MemoryStream(Convert.FromBase64String(base64text)))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
