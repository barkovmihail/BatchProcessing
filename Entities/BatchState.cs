using System;
using System.Collections.Generic;
using System.Threading;

namespace BatchProcessing.Entities
{
    public class BatchState
    {
        public CancellationTokenSource Token { get; set; }

        public Dictionary<Guid, Action> Actions { get; set; }
    }
}
