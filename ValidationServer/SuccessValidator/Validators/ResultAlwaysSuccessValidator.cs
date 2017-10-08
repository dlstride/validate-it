using System.Collections.Generic;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace SuccessValidator.Validators
{
    public class ResultAlwaysSuccessValidator : Validator, IValidator
    {
        public ResultAlwaysSuccessValidator() :
            base(SuccessValidatorProxy.SuccessValidatorId)
        {
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Info("SuccessValidator Execute started");

            List<IValidatorResult> validatorResults = new List<IValidatorResult>();

            var vr = new ValidatorResult(this.ValidatorProxyId, "SuccessValidator always returns result code of success ", ValidatorResultCode.Success , "SuccessValidator");
            validatorResults.Add(vr);

            log.Info("Starting SuccessValidator Execute completed with {ResultCount} results", validatorResults.Count);

            return validatorResults;
        }
    }
}