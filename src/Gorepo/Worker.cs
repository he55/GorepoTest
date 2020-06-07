using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gorepo.Data;
using Gorepo.Models;
using Gorepo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gorepo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly WeChatMessageService _messageService;
        private int _timestamp;

        public Worker(ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            WeChatMessageService messageService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _messageService = messageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
                            hwzContext.Messages.Add(new HWZMessage
                            {
                                CreateTime = message.Timestamp,
                                ServerId = message.MessageId,
                                Content = message.Message,
                                OrderId = $"id_{message.MessageId}"
                            });
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
