using System;
using System.Threading.Tasks;
using Gorepo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gorepo.Controllers
{
    public class SourceController : ControllerBase
    {
        private static string? _appUrl;

        public string AppUrl => _appUrl!;

        public SourceController(IConfiguration configuration)
        {
            if (_appUrl == null)
            {
                _appUrl = configuration.GetValue<string>("App:AppUrl");
            }
        }

        [HttpGet("release")]
        [HttpGet("packages")]
        public IActionResult GetSource()
        {
            return File(Request.Path, "text/plain; charset=utf-8");
        }

        [HttpGet("mobileconfig")]
        public async Task<IActionResult> GetMobileConfigAsync()
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string url = $"{AppUrl}/tools/udid?timestamp={timestamp}";

            string mobileConfig = await AppleMobileConfig.MakeMobileConfigAsync(url, timestamp);
            return Content(mobileConfig, "application/x-apple-aspen-config");
        }
    }
}
