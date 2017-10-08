using Ninject;
using Ninject.Parameters;
using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using ValidationServer.Data.Validators;

namespace LongRunningValidator.Validators
{
    public class LongRunningValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string LongRunningValidatorId = "LongRunningValidator";

        public const string LongRunningValidatorName = "Long Running Validator";

        public const string LongRunningValidatorDescription = "Long Running Validator: Allow QE to configure a validator with different run times.";

        public LongRunningValidatorProxy(IKernel childKernel)
            : base(childKernel, LongRunningValidatorId, LongRunningValidatorName, LongRunningValidatorDescription)
        {
        }

        protected override void LoadValidatonInstances(List<ConfigSection> configValues)
        {
            if (this._validatorInstances == null)
            {
                this._validatorInstances = new Dictionary<IValidatorInstance, ValidatorSettings>();

                LongRunningValidatorSettings settings = new LongRunningValidatorSettings(configValues);

                var validatorInstance = new ValidatorInstance(this.ValidatorId, this.Name, this.Description);

                var validatorInstances = new List<ValidatorInstance>() { validatorInstance };

                log.Debug("LongRunningValidator: {count} ValidatorInstances found available.", validatorInstances.Count);

                _validatorInstances.Add(validatorInstance, settings);
            }
        }

        public IValidator Create(IValidatorInstance validatorInstance)
        {
            LongRunningValidatorSettings settings = (LongRunningValidatorSettings)this._validatorInstances[validatorInstance];

            if (settings == null)
            {
                throw new Exception(string.Format("Long Running Validator Instance not found: {0}", validatorInstance.Name));
            }

            log.Debug("LongRunningValidator: Creating LongRunningValidator ValidatorInstance  Name: {name}  Description: {description}", validatorInstance.Name, validatorInstance.Description);

            return this._childKernel.Get<LongRunningConfigurableValidator>
                             (new ConstructorArgument(LongRunningConfigurableValidator.LongRunningSettingsConstructorParameterName, settings));
        }
    }
}