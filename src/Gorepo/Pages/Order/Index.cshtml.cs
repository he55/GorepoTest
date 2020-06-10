using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gorepo.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly GorepoContext _context;

        public IndexModel(GorepoContext context)
        {
            _context = context;
        }

        public IList<HWZWeChatOrder> HWZWeChatOrder { get; set; } = null!;

        public async Task OnGetAsync()
        {
            HWZWeChatOrder = await _context.WeChatOrders.ToListAsync();
        }
    }
}
