using ValidationServer.Scheduler.Scheduler;

namespace ValidationServer.Interfaces
{
    public interface IValidatorRunner
    {
        void QueueValidatorForExecution(ValidatorInstanceInfo info);

        event ReturnValidatorResults ValidatorResultsReady;
    }
}