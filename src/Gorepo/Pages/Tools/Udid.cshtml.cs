using System;
using System.IO;
using System.Threading.Tasks;
using Gorepo.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Gorepo.Pages
{
    [IgnoreAntiforgeryToken]
    public class UdidModel : PageModel
    {
        private const string TimestampCookieKey = "timestamp";
        private const double TimestampCookieTimeoutSeconds = 60.0;
        private const string PlistKeyFormat = "plist_{0}";
        private const string PlistPathFormat = "plist/{0}.data";
        private const double PlistCacheTimeoutSeconds = 100.0;

        private readonly IMemoryCache _memoryCache;

        public string? PlistString { get; set; }
        public PlistDictionary? PlistDictionary { get; set; }

        public UdidModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult OnGet(string id = "")
        {
            if (!Request.Cookies.ContainsKey(TimestampCookieKey))
            {
                Response.Cookies.Append(
                    TimestampCookieKey,
                    DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                    new CookieOptions
                    {
                        MaxAge = TimeSpan.FromSeconds(TimestampCookieTimeoutSeconds)
                    }
                );
            }

            if (Request.Headers["User-Agent"].Equals("Profile/1.0"))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            if (!string.IsNullOrWhiteSpace(id) &&
                _memoryCache.TryGetValue<string>(string.Format(PlistKeyFormat, id), out string plistString))
            {
                PlistString = plistString;
                PlistDictionary = PlistConfig.GetPlistConfigModel(plistString);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Request.ContentType.Equals("application/pkcs7-signature", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage();
            }

            string id = Guid.NewGuid().ToString();
            using (FileStream fileStream = new FileStream(string.Format(PlistPathFormat, id), FileMode.Create))
            {
                await Request.Body.CopyToAsync(fileStream);
                fileStream.Position = 0;

                if (PlistConfig.TryGetPlistString(fileStream, out string plistString))
                {
                    _memoryCache.Set(
                        string.Format(PlistKeyFormat, id),
                        plistString,
                        TimeSpan.FromSeconds(PlistCacheTimeoutSeconds)
                    );
                }
            }

            return RedirectToPagePermanent("", new { id });
        }
    }
}
