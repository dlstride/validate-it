namespace ValidationServer.Scheduler
{
    public interface IScheduleCallback
    {
        event CallBackToValidationServer RunValidatorEvent;

        void ScheduleValidator(string jobId, string cronTabExpression);
    }
}
