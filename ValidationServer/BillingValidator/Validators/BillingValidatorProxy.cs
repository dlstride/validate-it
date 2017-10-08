using Ninject;
using Ninject.Parameters;
using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using ValidationServer.Data.Validators;

namespace BillingValidator.Validators
{
    public class BillingValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string BillingValidatorId = "BillingValidator";

        public const string BillingValidatorName = "Billing Validator";

        public const string BillingValidatorDescription = "Billing Validator: Make sure Billing ran";

        public BillingValidatorProxy(IKernel childKernel, IVasaClient vasa) : base(childKernel, BillingValidatorId, BillingValidatorName, BillingValidatorDescription)
        {
        }

        protected override void LoadValidatonInstances(List<ConfigSection> configValues)
        {
            if (this._validatorInstances == null)
            {
                this._validatorInstances = new Dictionary<IValidatorInstance, ValidatorSettings>();

                BillingValidatorSettings settings = new BillingValidatorSettings(configValues);

                var validatorInstance = new ValidatorInstance(this.ValidatorId, this.Name, this.Description);

                //Billing validator only ever one instance
                var validatorInstances = new List<ValidatorInstance>() { validatorInstance };

                log.Debug("BillingValidator: {count} ValidatorInstances found available.", validatorInstances.Count);

                _validatorInstances.Add(validatorInstance, settings);
            }
        }


        public IValidator Create(IValidatorInstance validatorInstance)
        {
            BillingValidatorSettings settings = (BillingValidatorSettings)this._validatorInstances[validatorInstance];

            if (settings == null)
            {
                throw new Exception(string.Format("Billing Validator Instance not found: {0}", validatorInstance.Name));
            }

            log.Debug("BillingValidator: Creating BillingValidator ValidatorInstance  Name: {name}  Description: {description}", validatorInstance.Name, validatorInstance.Description);

            return this._childKernel.Get<BillingRanValidator>(new ConstructorArgument(BillingRanValidator.BillingSettingsConstructorParameterName, settings));
        }
    }
}