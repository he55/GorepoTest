using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gorepo.Pages.Order
{
    public class DeleteModel : PageModel
    {
        private readonly GorepoContext _context;

        public DeleteModel(GorepoContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HWZWeChatOrder = await _context.WeChatOrders.FindAsync(id);

            if (HWZWeChatOrder != null)
            {
                _context.WeChatOrders.Remove(HWZWeChatOrder);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
