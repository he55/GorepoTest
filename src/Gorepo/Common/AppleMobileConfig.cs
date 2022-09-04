using System.IO;
using System.Text;

namespace Gorepo.Common
{
    public static class AppleMobileConfig
    {
        static readonly string s_mobileConfigTemplate = File.ReadAllText("mobileconfig");

        public static string MakeMobileConfigAsync(string url, string challenge)
        {
            StringBuilder stringBuilder = new StringBuilder(s_mobileConfigTemplate);
            stringBuilder.Replace("{{URL}}", url);
            stringBuilder.Replace("{{Challenge}}", challenge);

            return stringBuilder.ToString();
        }
    }
}
