using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Data.Validators;

namespace ValidationSchedulerData
{
    //Input can be JSON request, file path - parser will be in charge of actual logic behind parsing input.
    // is this class necessary?
    public interface ISchedulerInput
    {
        string Input { get; }
    }
}
