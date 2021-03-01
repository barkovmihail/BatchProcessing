using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcessing.Core.Interfaces
{
    public interface IBatchManager
    {
        /// <summary>
        /// Runs batch calculation
        /// </summary>
        /// <param name="actions">List of action for calculation</param>
        /// <param name="maxDegreeOfParallelism">Parallelism degree</param>
        /// <returns>Operation identifier</returns>
        Task<Guid> RunProcessing(
            Dictionary<Guid, Action> actions,
            int maxDegreeOfParallelism = 0);

        Task<bool> AbortProcessing(Guid processingId);

        Task<Guid> GetCurrentProcessingId();

        CancellationTokenSource CurrentCancellationToken(Guid processingId);
    }
}