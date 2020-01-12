using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Gorepo.Controllers
{
    public class SourceFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SourceFileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("/{file}")]
        public IActionResult GetFile(string file)
        {
            if (!file.Equals("release", StringComparison.OrdinalIgnoreCase) &&
                !file.Equals("packages", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            IFileInfo fileInfo = _webHostEnvironment.WebRootFileProvider.GetFileInfo(file);
            if (!fileInfo.Exists)
            {
                return NotFound();
            }

            return File(file, "text/plain; charset=utf-8");
        }
    }
}
