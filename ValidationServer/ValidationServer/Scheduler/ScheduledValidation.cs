using PrecisionDiscovery.Diagnostics;
using System;
using ValidationServer.Data.Scheduler;

namespace ValidationSchedulerData.Scheduler
{
    public class ScheduledValidation : IScheduledValidation
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ScheduledValidation(string jobId, string cronTabExpression, DateTime? createdAt, DateTime? lastExecution = null, DateTime? nextExecution = null)
        {
            this.JobId = Guard.NotNullOrEmpty(jobId, "jobId", log);
            this.CronTabExpression = Guard.NotNullOrEmpty(cronTabExpression, "cronTabExpression", log);
            this.CreateAt = createdAt;
            this.LastExecution = lastExecution;
            this.NextExecution = nextExecution;
        }

        public DateTime? CreateAt { get; }

        public string CronTabExpression { get; }

        public string JobId { get; }

        public DateTime? LastExecution { get; }

        public DateTime? NextExecution { get; }
    }
}
