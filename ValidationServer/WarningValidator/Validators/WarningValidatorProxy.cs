using Ninject;
using PrecisionDiscovery.Diagnostics;
using ValidationServer.Data.Validators;

namespace WarningValidator.Validators
{
    public class WarningValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string WarningValidatorId = "WarningValidator";

        public const string WarningValidatorName = "Warning Validator";

        public const string WarningValidatorDescription = "Warning Validator: Always return a warningful result";


        public WarningValidatorProxy(IKernel childKernel) : base(childKernel, WarningValidatorId, WarningValidatorName, WarningValidatorDescription)
        {
        }

        public IValidator Create(IValidatorInstance validatorInstance)
        {
            Guard.NotNull(validatorInstance, "validatorInstance", log);

            return this._childKernel.Get<ResultAlwaysWarningValidator>();
        }
    }
}