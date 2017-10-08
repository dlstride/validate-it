using System.Collections.Generic;
using ValidationServer.Data.Validators;

namespace ValidationServer.Interfaces
{
    public interface IValidatorProvider
    {
        /*TODO - work on descriptions
            Figure out how and give me a list of all the proxies 
            Might want to consider this to be a Instance provider 
            Give back actual instances of the Validator when asked for 
        */

        IEnumerable<IValidatorProxy> ValidatorProxies { get; }

        IValidatorProxy GetValidatorProxy(string validationId);

        IValidatorInstance GetValidatorInstance(string validationId, string validatorInstanceId);

        //TODO ? add create instance...

    }
}