using System.Collections.Generic;
using ValidationServer.Data.Scheduler;
using ValidationServer.Scheduler.Scheduler;

namespace ValidationServer.Interfaces
{
    public interface IValidationServer    {

        void QueueValidatorForExecution(ValidatorInstanceInfo info);


        //External API

        //TODO - add other crud methods

        IEnumerable<IScheduledValidation> AddValidationsToSchedule(IEnumerable<IScheduleRequest> scheduleRequests);
    }
}
