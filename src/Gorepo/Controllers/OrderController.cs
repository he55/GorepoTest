using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gorepo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Gorepo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly HWZContext _context;
        private readonly HttpClient _httpClient;

        public OrderController(HWZContext context,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient("wed");
        }

        [HttpGet("{orderId}")]
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

        [HttpPost]
        public async Task<ActionResult<object>> PostOrderAsync(string orderId, decimal orderAmount)
        {
            return await _httpClient.GetStringAsync($"api/make_order?orderId={orderId}&orderAmount={orderAmount}");
        }
    }
}
