using System;
using System.Linq;
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

        private int _timestamp;

        public Worker(ILogger<Worker> logger, WeChatMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timestamp = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    WeChatMessageItem[] messages = await _messageService.GetWeChatMessageItemsAsync(_timestamp);
                    if (messages.Length > 0)
                    {
                        _timestamp = messages.Max(x => x.Timestamp);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{message}", ex.Message);
                }

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
