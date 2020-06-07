using System;
using System.Threading;
using System.Threading.Tasks;
using Gorepo.Models;
using Gorepo.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gorepo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly WeChatMessageService _messageService;

        public Worker(ILogger<Worker> logger, WeChatMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    WeChatMessageItem[] weChatMessageItems = await _messageService.GetWeChatMessageItemsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                }

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
