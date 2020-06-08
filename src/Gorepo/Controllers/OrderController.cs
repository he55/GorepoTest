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
            var message = _context.Messages
                .Where(m => m.OrderId == orderId)
                .Select(m => new
                {
                    m.OrderId,
                    m.OrderAmount
                })
                .FirstOrDefault();

            if (message == null)
            {
                return NotFound();
            }
            return message;
        }
    }
}
