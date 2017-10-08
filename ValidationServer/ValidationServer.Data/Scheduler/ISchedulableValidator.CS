namespace ValidationServer.Data.Scheduler
{

    //TODO work on name, this is pretty close to IScheduledValidation
    public interface ISchedulableValidator
    {
        string ValidatorId { get; }
        string ValidatorInstanceId { get; }

        bool IsScheduled { get; }

        //If the validator is scheduled then ScheduledValidation info will be set
        IScheduledValidation ScheduledValidation { get; }
    }
}
