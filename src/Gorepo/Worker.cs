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
                HWZContext hwzContext = _serviceProvider.CreateScope()
                    .ServiceProvider
                    .GetRequiredService<HWZContext>();

                await hwzContext.Database.EnsureCreatedAsync();

                _timestamp = hwzContext.Messages.Max(x => (int?)x.CreateTime) ?? 0;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    WeChatMessageItem[] messages = await _messageService.GetWeChatMessageItemsAsync(_timestamp);

                    if (messages.Length > 0)
                    {
                        HWZContext hwzContext = _serviceProvider.CreateScope()
                            .ServiceProvider
                            .GetRequiredService<HWZContext>();

                        foreach (WeChatMessageItem message in messages)
                        {
                            Dictionary<string, string> messageInfo = _messageService.GetMessageInfo(message.Message);

                            string orderId = messageInfo["detail_content_value_1"];
                            orderId = orderIdPrefix + message.MessageId;

                            if (orderId.StartsWith(orderIdPrefix) &&
                                decimal.TryParse(messageInfo["detail_content_value_0"].Replace("гд", ""), out decimal amount))
                            {
                                hwzContext.Messages.Add(new HWZMessage
                                {
                                    ServerId = message.MessageId,
                                    CreateTime = message.Timestamp,
                                    Content = message.Message,
                                    OrderId = orderId,
                                    OrderAmount = amount
                                });
                            }
                        }

                        await hwzContext.SaveChangesAsync();

                        _timestamp = messages.Max(x => x.Timestamp);
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
