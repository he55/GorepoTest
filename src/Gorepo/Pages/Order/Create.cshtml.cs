using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gorepo.Pages.Order
{
    public class CreateModel : PageModel
    {
        private readonly HWZGorepoContext _context;

        public CreateModel(HWZGorepoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public HWZWeChatOrder HWZWeChatOrder { get; set; } = null!;

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.WeChatOrders.Add(HWZWeChatOrder);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
