﻿namespace ValidationSchedulerData
{
    public interface IScheduleRequest
    {
        string ValidatorId { get; }

        string ValidatorInstanceId { get; }

        string CronTabExpression { get; }
    }
}
