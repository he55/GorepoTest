using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gorepo
{
    public abstract class BackgroundTask
    {
        private bool _runTaskFlag;
        private bool _taskRequestFlag;
        private readonly object _lock = new object();

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public void RequestTask()
        {
            // 不要锁住
            if (_runTaskFlag)
            {
                _taskRequestFlag = true;
                Console.WriteLine("Now runing!!!");
                return;
            }


            lock (_lock)
            {
                Task.Run(async () =>
                {
                    _runTaskFlag = true;

                    do
                    {
                        _taskRequestFlag = false;
                        await ExecuteAsync(default);
                    } while (_taskRequestFlag);

                    _runTaskFlag = false;
                });
            }
        }
    }
}
