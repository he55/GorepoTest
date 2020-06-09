using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gorepo
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
        public async Task<ResultModel> GetOrderAsync(string orderId)
        {
            if (orderId.Length == 0)
            {
                return this.ResultFail("参数错误，订单号长度不能为 0");
            }

            var message = await _context.Messages
                .AsNoTracking()
                .Where(m => m.OrderId == orderId)
                .Select(m => new
                {
                    m.OrderId,
                    m.OrderAmount
                })
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return this.ResultFail("没有找到指定订单");
            }


            HWZOrder order = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .FirstOrDefaultAsync();

            if (order != null)
            {
                order.IsPay = true;
                order.UpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                await _context.SaveChangesAsync();
            }

            return this.ResultSuccess(message);
        }


        [HttpPost]
        public async Task<ResultModel> CreateOrderAsync(WeChatOrder order)
        {
            if (order.OrderAmount < 0.01m)
            {
                return this.ResultFail("参数错误，金额不能小于 0.01 元");
            }

            if (order.OrderId.Length == 0)
            {
                return this.ResultFail("参数错误，订单号长度不能为 0");
            }

            if (await _context.Orders.AnyAsync(o => o.OrderId == order.OrderId))
            {
                return this.ResultFail("订单号已经存在");
            }


            string code;
            try
            {
                code = await _httpClient.GetStringAsync(
                    $"api/make_order?orderId={order.OrderId}&orderAmount={order.OrderAmount}");
            }
            catch
            {
                return this.ResultFail("服务器内部错误，调用订单生成接口失败");
            }


            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            _context.Orders.Add(new HWZOrder
            {
                OrderId = order.OrderId,
                OrderAmount = order.OrderAmount,
                Code = code,
                CreateTime = timestamp,
                UpdateTime = timestamp
            });

            await _context.SaveChangesAsync();


            order.Code = code;
            return this.ResultSuccess(order);
        }
    }
}
