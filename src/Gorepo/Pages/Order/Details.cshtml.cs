using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gorepo.Pages.Order
{
    public class DetailsModel : PageModel
    {
        private readonly HWZGorepoContext _context;

        public DetailsModel(HWZGorepoContext context)
        {
            _context = context;
        }

        public HWZWeChatOrder HWZWeChatOrder { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HWZWeChatOrder = await _context.WeChatOrders.FirstOrDefaultAsync(m => m.Id == id);

            if (HWZWeChatOrder == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
