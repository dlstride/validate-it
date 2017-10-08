using System.Collections.Generic;
using ValidationSchedulerData.Interfaces;
using ValidationServer.Data.Validators;

namespace ValidationSchedulerData
{
    public interface IValidationScheduler
    {
        //In charge of adding to calendar and cleaning up schedules (are they still valid, 
        //remove schedules, add schedules, check scheduled jobs)

        IScheduledValidation AddValidationToSchedule(IScheduleRequest scheduleRequest);

        IEnumerable<IScheduledValidation> AddValidationsToSchedule(IEnumerable<IScheduleRequest> scheduleRequests);

        IScheduledValidation EditValidationSchedule(IScheduleRequest scheduleRequest);

        IEnumerable<IScheduledValidation> GetScheduledValidations();

        IEnumerable<ISchedulableValidator> GetValidatorsAvailableToBeScheduled();

        IEnumerable<IValidatorProxy> GetValidatorsNotScheduled();

        //TODO: think about error conditions, only throw, maybe return bool... 
        void RemoveValidationFromSchedule(string validatorInstanceId);

        void RunValidator(string ValidatorId, string validatorInstanceId);

        //TODO: clean up scheduled validations that are no longer valid, for a future sprint
        //List<IScheduledValidation> CleanUpSchedule();
    }
}