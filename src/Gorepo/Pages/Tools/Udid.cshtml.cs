using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Gorepo.Pages
{
    [IgnoreAntiforgeryToken]
    public class UdidModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;

        public UdidModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public PlistDictionary? PlistDictionary { get; set; }

        public IActionResult OnGet(string id = "")
        {
            if (Request.Headers["User-Agent"].Equals("Profile/1.0"))
                return StatusCode(StatusCodes.Status204NoContent);

            if (!string.IsNullOrWhiteSpace(id) && _memoryCache.TryGetValue(id, out string plistString))
                PlistDictionary = AppleMobileConfig.GetPlistConfigModel(plistString);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Request.ContentType.Equals("application/pkcs7-signature", StringComparison.OrdinalIgnoreCase))
                return RedirectToPage();

            string id = Guid.NewGuid().ToString();

            using (FileStream fileStream = new FileStream(Path.Combine("plist", $"{id}.plist"), FileMode.Create))
            {
                await Request.Body.CopyToAsync(fileStream);
                fileStream.Position = 0;

                if (AppleMobileConfig.TryGetPlistString(fileStream, out string plistString))
                    _memoryCache.Set(id, plistString, TimeSpan.FromSeconds(100.0));
            }

            return RedirectToPagePermanent("", new { id });
        }
    }
}
