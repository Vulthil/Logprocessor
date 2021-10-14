using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LogProcessor.HostedServices.Channels;
using LogProcessor.Interfaces;
using LogProcessor.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogProcessor.HostedServices
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, IServiceScopeFactory, Task> workItem);
        Task<Func<CancellationToken, IServiceScopeFactory, Task>> DequeueAsync(
            CancellationToken cancellationToken);

        int CountItems();
    }
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, IServiceScopeFactory, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, IServiceScopeFactory, Task>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, IServiceScopeFactory, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, IServiceScopeFactory, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public int CountItems()
        {
            return _workItems.Count;
        }
    }

    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly IChannelReader<ErrorResult> _channelReader;
        private readonly List<ErrorResult> _batch = new();
        private readonly SemaphoreSlim _signal = new(1);
        private CancellationToken _stoppingToken;
        private Timer _flushScheduler;

        public QueuedHostedService(
            ILogger<QueuedHostedService> logger,
            IChannelReader<ErrorResult> channelReader,
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _channelReader = channelReader;
        }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _flushScheduler = new Timer(TaskFlush, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            await BackgroundProcessing(stoppingToken);
        }


        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            await foreach (var item in _channelReader.ReadAllAsync(stoppingToken))
            {
                await _signal.WaitAsync(stoppingToken);
                _batch.Add(item);
                await CheckQueue(stoppingToken);
                _signal.Release();
            }
        }

        private async void TaskFlush(object state)
        {
            await _signal.WaitAsync(_stoppingToken);
            await FlushAsync();
            _signal.Release();
        }

        private async Task CheckQueue(CancellationToken stoppingToken)
        {
            if (_batch.Count > 100)
            {
                await FlushAsync();
            }
        }

        private async Task FlushAsync()
        {
            if (_batch.Count == 0)
            {
                return;
            }
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    await scope.ServiceProvider.GetRequiredService<IProtocolErrorHandler>().PersistErrors(_batch);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {item}."/*, nameof(item)*/);
                }

            }
            _batch.Clear();
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _flushScheduler?.Change(Timeout.Infinite, 0);
            await base.StopAsync(stoppingToken);
        }
        public override void Dispose()
        {
            _flushScheduler?.Dispose();
            base.Dispose();
        }
    }
}