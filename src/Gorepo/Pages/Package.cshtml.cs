using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Gorepo.Pages
{
    public class PackageModel : PageModel
    {
        private readonly ILogger<PackageModel> _logger;

        public PackageModel(ILogger<PackageModel> logger)
        {
            _logger = logger;
        }

        public string PackageId { get; set; } = string.Empty;

        public void OnGet(string packageId)
        {
            PackageId = packageId;
        }
    }
}
