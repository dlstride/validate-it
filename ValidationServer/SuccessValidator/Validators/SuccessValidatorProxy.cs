using Ninject;
using PrecisionDiscovery.Diagnostics;
using ValidationServer.Data.Validators;

namespace SuccessValidator.Validators
{
    class SuccessValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string SuccessValidatorId = "SuccessValidator";

        public const string SuccessValidatorName = "Success Validator";

        public const string SuccessValidatorDescription = "Success Validator: Always return a successful result";

        public SuccessValidatorProxy(IKernel childKernel) : base(childKernel, SuccessValidatorId, SuccessValidatorName, SuccessValidatorDescription)
        {
        }

        public IValidator Create(IValidatorInstance validatorInstance)
        {
            Guard.NotNull(validatorInstance, "validatorInstance", log);

            return this._childKernel.Get<ResultAlwaysSuccessValidator>();
        }
    }
}