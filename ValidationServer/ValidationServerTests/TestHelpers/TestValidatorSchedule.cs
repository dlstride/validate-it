using System.Collections.Generic;
using ValidationServer.Data.Scheduler;
using ValidationServer.Interfaces;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidatorSchedule : IValidationScheduler
    {
        public IEnumerable<IScheduledValidation> AddValidationsToSchedule(IEnumerable<IScheduleRequest> scheduleRequests)
        {
            return null;
        }

        public IScheduledValidation AddValidationToSchedule(IScheduleRequest scheduleRequest)
        {
            return null;
        }

        public IScheduledValidation EditValidationSchedule(IScheduleRequest scheduleRequest)
        {
            return null;
        }

        public IEnumerable<IScheduledValidation> GetScheduledValidations()
        {
            return null;
        }

        public IEnumerable<IScheduledValidation> GetScheduledValidations(int startAt, int endAt)
        {
            return null;
        }

        public bool IsValidationScheduled(string ValidatorId, string validatorInstanceId)
        {
            return true;
        }

        public void RemoveValidationFromSchedule(string ValidatorId, string validatorInstanceId)
        {
           
        }

        public void TriggerJob(string validatorId, string validatorInstanceId)
        {

        }
    }
}