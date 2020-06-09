using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gorepo.Pages.Order
{
    public class EditModel : PageModel
    {
        private readonly HWZGorepoContext _context;

        public EditModel(HWZGorepoContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(HWZWeChatOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HWZWeChatOrderExists(HWZWeChatOrder.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool HWZWeChatOrderExists(int id)
        {
            return _context.WeChatOrders.Any(e => e.Id == id);
        }
    }
}
