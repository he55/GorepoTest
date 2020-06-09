using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gorepo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly WeChatMessageService _messageService;
        private int _timestamp;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            WeChatMessageService messageService)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _messageService = messageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string orderIdPrefix = _configuration.GetValue<string>("App:OrderIdPrefix");

            {
                HWZGorepoContext context = _serviceProvider.CreateScope()
                    .ServiceProvider
                    .GetRequiredService<HWZGorepoContext>();

                await context.Database.EnsureCreatedAsync();

                _timestamp = context.WeChatMessages.Max(x => (int?)x.MessageCreateTime) ?? 0;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10_000, stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);


                WeChatMessage[] messages;
                try
                {
                    messages = await _messageService.GetWeChatMessagesAsync(_timestamp);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{message}", ex.Message);
                    continue;
                }


                if (messages.Length == 0)
                {
                    continue;
                }


                HWZGorepoContext context = _serviceProvider.CreateScope()
                    .ServiceProvider
                    .GetRequiredService<HWZGorepoContext>();

                foreach (WeChatMessage message in messages)
                {
                    Dictionary<string, string> messageInfo = _messageService.GetMessageInfo(message.Message);

                    string orderId = messageInfo["detail_content_value_1"];
                    orderId = orderIdPrefix + message.ServerId;

                    if (orderId.StartsWith(orderIdPrefix) &&
                        decimal.TryParse(messageInfo["detail_content_value_0"].Replace("\uffe5", ""), out decimal amount))
                    {
                        long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                        context.WeChatMessages.Add(new HWZWeChatMessage
                        {
                            MessageCreateTime = message.CreateTime,
                            MessageId = message.ServerId,
                            MessageContent = message.Message,
                            MessagePublishTime = int.TryParse(messageInfo["header_pub_time"], out int publishTime) ? publishTime : 0,
                            OrderId = orderId,
                            OrderAmount = amount,
                            CreateTime = timestamp,
                            UpdateTime = timestamp
                        });

                        await context.SaveChangesAsync();
                        _timestamp = message.CreateTime;
                    }
                }
            }
        }
    }
}
