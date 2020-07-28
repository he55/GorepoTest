using Microsoft.AspNetCore.StaticFiles;

namespace Gorepo.Common
{
    public class HWZFileExtensionContentTypeProvider : FileExtensionContentTypeProvider
    {
        public HWZFileExtensionContentTypeProvider()
        {
            Mappings[".plist"] = "application/octet-stream";
            Mappings[".ipa"] = "application/octet-stream";
            Mappings[".mobileconfig"] = "application/x-apple-aspen-config";
            Mappings[".bz2"] = "application/x-bzip2";
            Mappings[".deb"] = "application/vnd.debian.binary-package";
        }
    }
}
