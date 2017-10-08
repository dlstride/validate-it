using System.Collections.Generic;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace ErrorValidator.Validators
{
    class ResultAlwaysErrorValidator : Validator, IValidator
    {
        public ResultAlwaysErrorValidator() :
            base(ErrorValidatorProxy.ErrorValidatorId)
        {
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Info("ErrorValidator Execute started");

            List<IValidatorResult> validatorResults = new List<IValidatorResult>();

            var vr = new ValidatorResult(this.ValidatorProxyId, "ErrorValidator always returns result code of error ", ValidatorResultCode.Error, "ErrorValidator");
            validatorResults.Add(vr);

            log.Info("Starting ErrorValidator Execute completed with {ResultCount} results", validatorResults.Count);

            return validatorResults;
        }
    }
}