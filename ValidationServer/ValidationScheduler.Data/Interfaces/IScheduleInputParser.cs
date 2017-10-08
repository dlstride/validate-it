using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationSchedulerData
{
    public interface IScheduleInputParser
    {
        //parses schedule input. text parser, json parser, any other types of input
        // and returns an appropriately formatted request
        IEnumerable<IScheduleRequest> ParseAndCreateScheduleRequest();
    }

}
