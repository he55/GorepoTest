using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gorepo.Data;
using Gorepo.Models;
using Gorepo.Services;
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
        private int _lastCreateTime;

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
                HWZContext context = _serviceProvider.CreateScope()
                    .ServiceProvider
                    .GetRequiredService<HWZContext>();

                await context.Database.EnsureCreatedAsync();

                _lastCreateTime = context.Messages.Max(x => (int?)x.CreateTime) ?? 0;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    WeChatMessage[] messages = await _messageService.GetWeChatMessagesAsync(_lastCreateTime);

                    if (messages.Length > 0)
                    {
                        HWZContext context = _serviceProvider.CreateScope()
                            .ServiceProvider
                            .GetRequiredService<HWZContext>();

                        foreach (WeChatMessage message in messages)
                        {
                            Dictionary<string, string> messageInfo = _messageService.GetMessageInfo(message.Message);

                            string orderId = messageInfo["detail_content_value_1"];
                            orderId = orderIdPrefix + message.ServerId;

                            if (orderId.StartsWith(orderIdPrefix) &&
                                decimal.TryParse(messageInfo["detail_content_value_0"].Replace("\uffe5", ""), out decimal amount))
                            {
                                context.Messages.Add(new HWZMessage
                                {
                                    CreateTime = message.CreateTime,
                                    ServerId = message.ServerId,
                                    Message = message.Message,
                                    PublishTime = int.TryParse(messageInfo["header_pub_time"], out int publishTime) ? publishTime : 0,
                                    OrderId = orderId,
                                    OrderAmount = amount
                                });
                            }
                        }

                        await context.SaveChangesAsync();

                        _lastCreateTime = messages.Max(x => x.CreateTime);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{message}", ex.Message);
                }

                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}
