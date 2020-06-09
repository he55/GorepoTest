using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Gorepo.Data;
using Gorepo.Models;
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
        public ResultModel GetOrder(string orderId)
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
                return this.ResultFail("没有找到指定订单");
            }

            return this.ResultSuccess(message);
        }

        [HttpPost]
        public async Task<ResultModel> PostOrderAsync(WeChatOrder order)
        {
            if (order.OrderAmount < 0.01m)
            {
                return this.ResultFail("参数错误，金额不能小于 0.01 元");
            }

            if (order.OrderId.Length == 0)
            {
                return this.ResultFail("参数错误，订单号长度不能为 0");
            }

            try
            {
                string code = await _httpClient.GetStringAsync(
                    $"api/make_order?orderId={order.OrderId}&orderAmount={order.OrderAmount}");

                order.Code = code;

                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                _context.Orders.Add(new HWZOrder
                {
                    OrderId = order.OrderId,
                    OrderAmount = order.OrderAmount,
                    Code = order.Code,
                    CreateTime = timestamp,
                    UpdateTime = timestamp
                });

                await _context.SaveChangesAsync();

                return this.ResultSuccess(order);
            }
            catch
            {
                return this.ResultFail("服务器内部错误，调用订单生成接口失败");
            }
        }
    }
}
