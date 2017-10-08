using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationSchedulerData
{
    public interface IValidationCalendar
    {
        // holds all scheduledValidations  so that people can use it to grab them
        List<IScheduledValidation> GetAllScheduledValidations();

        List<IScheduledValidation> GetValidationsScheduled(DateTime date);

        List<IScheduledValidation> GetValidationsScheduled(DateTime startDate, DateTime endDate);
    }
}
