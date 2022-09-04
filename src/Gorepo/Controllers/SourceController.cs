using System;
using Gorepo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gorepo.Controllers
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
        public IActionResult GetMobileConfig()
        {
            string appUrl = _configuration.GetValue<string>("App:AppUrl");
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string url = $"{appUrl}/tools/udid?t={timestamp}";

            string mobileConfig = AppleMobileConfig.MakeMobileConfig(url, timestamp);
            return Content(mobileConfig, "application/x-apple-aspen-config");
        }
    }
}
