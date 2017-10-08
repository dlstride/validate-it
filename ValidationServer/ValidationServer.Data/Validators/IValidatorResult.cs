using System.Collections.Generic;
using ValidationServer.Data.Enumerations;

namespace ValidationServer.Data.Validators
{
    public interface IValidatorResult
    {
        string ValidatorId { get; }

        // A unique identifier that is consistent across runs
        //eg {nameofqueue}
        string ResultIdentifier { get; }

        string Description { get; }

        //TODO - Possibly new name more representative of what it is and used for
        IList<string> FilterSequence { get; }

        ValidatorResultCode ResultCode { get; }
    }
}