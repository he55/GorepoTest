using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gorepo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HWZOrderService _orderService;

        public Worker(ILogger<Worker> logger, HWZOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10_000, stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _orderService.SaveOrderAsync();
            }
        }
    }
}
