using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;

namespace ValidationServer.Data.Validators
{
    public interface IValidatorProxy
    {
        //TODO think about new name instead of proxy - > proxy responsible for .... validator metadata, discovery and creation
        string ValidatorId { get; }

        string Description { get; }

        Version Version { get; }

        IEnumerable<IValidatorInstance> GetAvailableValidators(List<ConfigSection> configValues);

        IEnumerable<IValidatorInstance> ValidatorInstances { get; }

        /*TODO:  For now this looks OK, we might have to revisit this when we get to implementing the scheduler, or reading schedules 
        from disk or some other place.  My concern is that the whole object might be hard to recreate.  Also makes it harder to make 
        sure implementers do the right thing with it (for example don't just do a reference compare)  We are still going to need the 
        ValidatorInstance for all the meta, maybe adding a string token to it for identification.*/
        IValidator Create(IValidatorInstance validatorInstance);
    }
} 