using System.Collections.Generic;

namespace ValidationServer.Data.Validators
{
    public interface IValidator
    {
        //TODO - Possibly new name more representative of what it is and used for

        IList<string> FilterSequence { get; }

        IList<IValidatorResult> Execute(IValidatorContext context);
    }
}