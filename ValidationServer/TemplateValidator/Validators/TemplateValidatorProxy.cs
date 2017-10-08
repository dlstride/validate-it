using Ninject;
using PrecisionDiscovery.Diagnostics;
using ValidationServer.Data.Validators;

namespace TemplateValidator.Validators
{
    public class TemplateValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string TemplateValidatorId = "TemplateValidator";

        public const string TemplateValidatorName = "Template Validator";

        public const string TemplateValidatorDescription = "Template Validator: template expected for validator";

        public TemplateValidatorProxy(IKernel childKernel)
            : base(childKernel, TemplateValidatorId, TemplateValidatorName, TemplateValidatorDescription)
        {
        }

        public IValidator Create(IValidatorInstance validatorInstance)
        {
            Guard.NotNull(validatorInstance, "validatorInstance", log);

            return this._childKernel.Get<TemplateTestValidator>();
        }
    }
}