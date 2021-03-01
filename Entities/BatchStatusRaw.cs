using System;
using BatchProcessing.Entities.Enums;

namespace BatchProcessing.Entities
{
    public class BatchStatusRaw
    {
        public long Id { get; set; }

        public string JobName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastRun { get; set; }

        public Status StatusId { get; set; }

        public int CompletePercent { get; set; }
    }
}