using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gorepo
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly WeChatService _wechatService;

        private bool _flag;
        private int _timestamp;
        private string _orderIdPrefix = "";

        public OrderService(ILogger<OrderService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            WeChatService wechatService)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _wechatService = wechatService;
        }

        public async Task SaveOrderAsync()
        {
            GorepoContext context = _serviceProvider.CreateScope()
                .ServiceProvider
                .GetRequiredService<GorepoContext>();

            if (!_flag)
            {
                // TODO: Remove
                await context.Database.EnsureCreatedAsync();

                _flag = true;

                _timestamp = await context.WeChatMessages.MaxAsync(x => (int?)x.MessageCreateTime) ?? 0;
                _orderIdPrefix = _configuration.GetValue<string>("App:OrderIdPrefix");
            }


            WeChatMessage[] messages;
            try
            {
                messages = await _wechatService.GetMessagesAsync(_timestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "消息接口调用失败");
                return;
            }


            if (messages.Length == 0)
            {
                _logger.LogInformation("消息数组长度为 0");
                return;
            }


            foreach (WeChatMessage message in messages)
            {
                Dictionary<string, string> messageInfo = _wechatService.GetMessageInfo(message.Message);

                string orderId = messageInfo["detail_content_value_1"];
                orderId = _orderIdPrefix + message.MessageId;


                if (!orderId.StartsWith(_orderIdPrefix))
                {
                    _logger.LogWarning("消息 Id 不是指定前缀");
                    continue;
                }


                HWZWeChatOrder order = await context.WeChatOrders
                    .Where(o => o.OrderId == orderId)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    _logger.LogWarning("找不到对应订单");
                    _timestamp = message.CreateTime;
                    continue;
                }


                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                order.IsOrderPay = true;
                order.UpdateTime = timestamp;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "订单数据保存异常");
                    _timestamp = message.CreateTime;
                    return;
                }


                if (!decimal.TryParse(messageInfo["detail_content_value_0"].Replace("\uffe5", ""), out decimal amount))
                {
                    _logger.LogWarning("订单金额转换失败");
                    continue;
                }


                context.WeChatMessages.Add(new HWZWeChatMessage
                {
                    MessageCreateTime = message.CreateTime,
                    MessageId = message.MessageId,
                    MessageContent = message.Message,
                    MessagePublishTime = int.TryParse(messageInfo["header_pub_time"], out int publishTime) ? publishTime : 0,
                    OrderId = orderId,
                    OrderAmount = amount,
                    CreateTime = timestamp,
                    UpdateTime = timestamp
                });


                try
                {
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "消息数据保存异常");
                }
                _timestamp = message.CreateTime;
            }
        }
    }
}
