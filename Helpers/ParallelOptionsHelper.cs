using System;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcessing.Helpers
{
    public static class ParallelOptionsHelper
    {
        public static ParallelOptions CreateDefault(CancellationToken token, int maxDegreeOfParallelism = 0) =>
            new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = maxDegreeOfParallelism == 0 ? Environment.ProcessorCount / 2 : maxDegreeOfParallelism,
            };
    }
}
