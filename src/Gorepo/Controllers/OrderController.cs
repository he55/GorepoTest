﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gorepo
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly HWZGorepoContext _context;
        private readonly WeChatService _wechatService;

        public OrderController(HWZGorepoContext context, WeChatService wechatService)
        {
            _context = context;
            _wechatService = wechatService;
        }

        [HttpGet("{orderId}")]
        public async Task<ResultModel> GetOrderAsync(string orderId)
        {
            if (orderId.Length == 0)
            {
                return this.ResultFail("参数错误，订单号长度不能为 0");
            }

            HWZWeChatOrder order = await _context.WeChatOrders
                .Where(m => m.OrderId == orderId)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return this.ResultFail("没有找到指定订单");
            }
            return this.ResultSuccess(order);
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

            if (await _context.WeChatOrders.AnyAsync(o => o.OrderId == order.OrderId))
            {
                return this.ResultFail("订单号已经存在");
            }


            string orderCode;
            try
            {
                orderCode = await _wechatService.CreateOrderAsync(order.OrderId, order.OrderAmount);
            }
            catch
            {
                return this.ResultFail("服务器内部错误，调用订单生成接口失败");
            }


            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            _context.WeChatOrders.Add(new HWZWeChatOrder
            {
                OrderId = order.OrderId,
                OrderAmount = order.OrderAmount,
                OrderCode = orderCode,
                CreateTime = timestamp,
                UpdateTime = timestamp
            });

            await _context.SaveChangesAsync();


            order.OrderCode = orderCode;
            return this.ResultSuccess(order);
        }
    }
}
