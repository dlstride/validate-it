using System.Collections.Generic;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace WarningValidator.Validators
{
    public class ResultAlwaysWarningValidator : Validator, IValidator
    {
        public ResultAlwaysWarningValidator() :
            base(WarningValidatorProxy.WarningValidatorId)
        {
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Info("WarningValidator Execute started");

            List<IValidatorResult> validatorResults = new List<IValidatorResult>();

            var vr = new ValidatorResult(this.ValidatorProxyId, "WarningValidator always returns result code of warning ", ValidatorResultCode.Warning, "WarningValidator");
            validatorResults.Add(vr);

            log.Info("Starting WarningValidator Execute completed with {ResultCount} results", validatorResults.Count);

            return validatorResults;
        }
    }
}