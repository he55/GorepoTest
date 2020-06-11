using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gorepo
{
    public abstract class BackgroundTask
    {
        private bool _runTaskFlag;
        private readonly object _lock = new object();

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public void RequestExecute()
        {
            // 不要锁住
            if (_runTaskFlag)
            {
                Console.WriteLine("Now runing!!!");
                return;
            }


            lock (_lock)
            {
                Task.Run(async () =>
                {
                    _runTaskFlag = true;

                    await ExecuteAsync(default);

                    _runTaskFlag = false;
                });
            }
        }
    }
}
