using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gorepo
{
    public class SourceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SourceController(IConfiguration configuration)
        {
            _configuration = configuration;
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
            string appUrl = _configuration.GetValue<string>("App:AppUrl");
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string url = $"{appUrl}/tools/udid?t={timestamp}";

            string mobileConfig = await AppleMobileConfig.MakeMobileConfigAsync(url, timestamp);
            return Content(mobileConfig, "application/x-apple-aspen-config");
        }
    }
}
