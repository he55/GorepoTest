using System.Linq;
using Gorepo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Gorepo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly HWZContext _context;

        public OrderController(HWZContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<object> GetOrder(string orderId)
        {
            HWZMessage message = _context.Messages.FirstOrDefault(m => m.OrderId == orderId);
            if (message == null)
            {
                return NotFound();
            }
            return message;
        }
    }
}
