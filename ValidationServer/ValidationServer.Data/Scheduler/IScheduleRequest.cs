namespace ValidationServer.Data.Scheduler
{
    public interface IScheduleRequest
    {
        string ValidatorId { get; }

        string ValidatorInstanceId { get; }

        string CronTabExpression { get; }
    }
}
