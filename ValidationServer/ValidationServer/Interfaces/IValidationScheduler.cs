using System.Collections.Generic;
using ValidationServer.Data.Scheduler;

namespace ValidationServer.Interfaces
{
    public interface IValidationScheduler
    {
        IScheduledValidation AddValidationToSchedule(IScheduleRequest scheduleRequest);

        IEnumerable<IScheduledValidation> AddValidationsToSchedule(IEnumerable<IScheduleRequest> scheduleRequests);

        IScheduledValidation EditValidationSchedule(IScheduleRequest scheduleRequest);

        IEnumerable<IScheduledValidation> GetScheduledValidations();

        IEnumerable<IScheduledValidation> GetScheduledValidations(int startingFrom, int endingAt);

        bool IsValidationScheduled(string ValidatorId, string validatorInstanceId);

        void RemoveValidationFromSchedule(string ValidatorId, string validatorInstanceId);

        void TriggerJob(string validatorId, string validatorInstanceId);

        //TODO: clean up scheduled validations that are no longer valid, for a future sprint
        //List<IScheduledValidation> CleanUpSchedule();
    }
}