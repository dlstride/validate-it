using Ninject;
using PrecisionDiscovery.Diagnostics;
using ValidationServer.Data.Validators;

namespace ErrorValidator.Validators
{
    public class ErrorValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string ErrorValidatorId = "ErrorValidator";

        public const string ErrorValidatorName = "Error Validator";

        public const string ErrorValidatorDescription = "Error Validator: Always return an error result";

        public ErrorValidatorProxy(IKernel childKernel) : base(childKernel, ErrorValidatorId, ErrorValidatorName, ErrorValidatorDescription)
        {
        }

        public IValidator Create(IValidatorInstance validatorInstance)
        {
            Guard.NotNull(validatorInstance, "validatorInstance", log);

            return this._childKernel.Get<ResultAlwaysErrorValidator>();
        }
    }
}