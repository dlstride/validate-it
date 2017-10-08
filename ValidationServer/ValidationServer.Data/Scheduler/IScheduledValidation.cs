using System;

namespace ValidationServer.Data.Scheduler
{
    public interface IScheduledValidation
    {
        string JobId { get; }

        string CronTabExpression { get; }

        DateTime? LastExecution { get; }

        DateTime? CreateAt { get; }

        DateTime? NextExecution { get; }
    }   
}