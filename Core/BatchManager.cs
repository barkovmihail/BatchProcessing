using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatchProcessing.Core.Interfaces;
using BatchProcessing.Entities;
using BatchProcessing.Helpers;
using BatchProcessing.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BatchProcessing.Core
{
    public class BatchManager : IBatchManager
    {
        private readonly ILogger logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Dictionary<Guid, BatchState> state;

        private IBackgroundTaskQueue Queue { get; }

        public BatchManager(
            IBackgroundTaskQueue queue,
            ILogger<BatchManager> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
            this.state = new Dictionary<Guid, BatchState>();

            this.Queue = queue;
        }

        public async Task<Guid> RunProcessing(
            Dictionary<Guid, Action> actions,
            int maxDegreeOfParallelism = 0)
        {
            if (!actions.Any())
            {
                return Guid.Empty;
            }

            var jobId = Guid.NewGuid();

            this.state.Add(jobId, new BatchState
            {
                Actions = actions,
                Token = new CancellationTokenSource(),
            });

            this.Queue.QueueBackgroundWorkItem(async cToken =>
            {
                var currentState = this.CurrentState(jobId);

                this.logger.LogInformation(
                    $"Queued background task {jobId} is started.");

                using (var scope = this.serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var po = ParallelOptionsHelper.CreateDefault(currentState.Token.Token, maxDegreeOfParallelism);

                    try
                    {
                        Parallel.ForEach(
                            currentState.Actions,
                            po,
                            (action, state) =>
                            {
                                po.CancellationToken.ThrowIfCancellationRequested();
                                action.Value();
                            });
                    }
                    catch (OperationCanceledException e)
                    {
                        this.logger.LogError(e, $"The operation was сanceled by the user. Error: {e.Message}");
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, $"A common error for batch processing. Error: {ex.Message}");
                    }
                    finally
                    {
                        this.ClearState(jobId);
                    }
                }

                this.logger.LogInformation($"Queued background task {jobId} is complete.");
            });

            return jobId;
        }

        public Task<bool> AbortProcessing(Guid processingId)
        {
            var token = this.CurrentCancellationToken(processingId);

            if (token != null)
            {
                token.Cancel();

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public async Task<Guid> GetCurrentProcessingId()
        {
            var item = this.state.FirstOrDefault(it => !it.Value.Token.IsCancellationRequested && it.Value.Actions.Any());

            return await Task.FromResult(item.Key);
        }

        public CancellationTokenSource CurrentCancellationToken(Guid processingId)
        {
            return this.state.ContainsKey(processingId) ? this.state[processingId].Token : null;
        }

        private BatchState CurrentState(Guid processingId)
        {
            return this.state.ContainsKey(processingId) ? this.state[processingId] : null;
        }

        private void ClearState(Guid processingId)
        {
            if (this.state.ContainsKey(processingId))
            {
                var cancellationToken = this.CurrentCancellationToken(processingId);
                if (cancellationToken != null)
                {
                    cancellationToken.Dispose();
                }

                this.state.Remove(processingId);
            }
        }
    }
}