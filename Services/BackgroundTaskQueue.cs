﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BatchProcessing.Services.Abstract;

namespace BatchProcessing.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        private SemaphoreSlim signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            this.workItems.Enqueue(workItem);
            this.signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await this.signal.WaitAsync(cancellationToken);
            this.workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
