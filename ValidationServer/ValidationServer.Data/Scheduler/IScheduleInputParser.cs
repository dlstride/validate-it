
using System.Collections.Generic;

namespace ValidationServer.Data.Scheduler
{
    public interface IScheduleInputParser
    {
        IEnumerable<IScheduleRequest> ParseAndCreateScheduleRequest();
    }
}