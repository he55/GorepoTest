using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Gorepo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UdidController : ControllerBase
    {
        private const string PlistKeyFormat = "plist_{0}";

        private readonly IMemoryCache _memoryCache;


        public UdidController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        [HttpGet("{id}")]
        public IActionResult GetUdid(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) &&
                _memoryCache.TryGetValue<string>(string.Format(PlistKeyFormat, id), out string plistString))
            {
                return Content(plistString, "application/xml");
            }

            return NotFound();
        }
    }
}
