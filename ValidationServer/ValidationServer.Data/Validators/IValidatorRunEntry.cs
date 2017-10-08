using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationServer.Data.Validators
{
    public interface IValidatorRunEntry
    {
        DateTime StartTime { get; }

        DateTime FinishTime { get; }

        IValidatorResult Result { get; }

        //TODO - Possibly new name more representative of what it is and used for
        IList<string> FilterSequence { get; }
    }
}
