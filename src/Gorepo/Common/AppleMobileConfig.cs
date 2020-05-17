using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Gorepo.Common
{
    public static class AppleMobileConfig
    {
        private const string MobileConfigFileName = "mobileconfig";
        private static string? _mobileConfigTemplate;

        public static async Task<string> GetMobileConfigTemplateAsync()
        {
            if (_mobileConfigTemplate == null)
            {
                _mobileConfigTemplate = await File.ReadAllTextAsync(MobileConfigFileName);
            }
            return _mobileConfigTemplate;
        }

        public static async Task<string> MakeMobileConfigAsync(string url, string challenge)
        {
            StringBuilder stringBuilder = new StringBuilder(await GetMobileConfigTemplateAsync());
            stringBuilder.Replace("{{URL}}", url);
            stringBuilder.Replace("{{Challenge}}", challenge);

            return stringBuilder.ToString();
        }
    }
}
