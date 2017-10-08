using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Data.Enumerations;

namespace ValidationServer.Data.Validators
{
    public interface IValidatorRunHistory
    {
        //some sort of run information
        //object can be passed back to validation server in order to write this information back to DB

        List<IValidatorRunEntry> RunHistory { get; }

        List<IValidatorRunEntry> ReturnPreviousRuns(int maxResults, ValidatorResultCode maxErrorLevel);
    }
}
