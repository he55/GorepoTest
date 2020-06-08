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
        public object GetOrder(string orderId)
        {
            HWZMessage message = _context.Messages.FirstOrDefault(m => m.OrderId == orderId);
            return message;
        }
    }
}
