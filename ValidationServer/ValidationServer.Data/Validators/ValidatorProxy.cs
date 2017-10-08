using Ninject;
using PrecisionDiscovery.Configuration.Vasa;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ValidationServer.Data.Validators
{
    public class ValidatorProxy
    {
        protected static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        protected IKernel _childKernel { get; }

        protected string _validatorId { get; }

        protected string _validatorName{ get; }

        protected string _validatorDescription { get; }

        protected Dictionary<IValidatorInstance, ValidatorSettings> _validatorInstances { get; set; }

        public ValidatorProxy(IKernel childKernel,string validatorId, string validatorName, string validatorDescription)
        {
            this._childKernel = Guard.NotNull(childKernel, "childKernel", log);

            this._validatorId = Guard.NotNull(validatorId, "validatorId", log);
            this._validatorName = Guard.NotNull(validatorName, "validatorName", log);
            this._validatorDescription = Guard.NotNull(validatorDescription, "validatorDescription", log);
        }

        public string ValidatorId
        {
            get
            {
                return _validatorId;
            }
        }

        public string Name
        {
            get
            {
                return _validatorName;
            }
        }

        public string Description
        {
            get
            {
                return _validatorDescription;
            }
        }

        public Version Version
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                return assembly.GetName().Version;
            }
        }

        public IEnumerable<IValidatorInstance> ValidatorInstances
        {
            get
            {
                if (_validatorInstances == null)
                {
                    return new List<IValidatorInstance>();
                }

                return _validatorInstances.Keys;
            }
        }

        public IEnumerable<IValidatorInstance> GetAvailableValidators(List<ConfigSection> configValues)
        {
            log.Info(string.Format("{0} GetAvailableValidators starting",this.ValidatorId));

            this.LoadValidatonInstances(configValues);

            IEnumerable<IValidatorInstance> validatorInstances = _validatorInstances.Keys.ToList();

            log.Info("{ValidatorId} GetAvailableValidators: {ValidatorInstanceCount} instances of the {0} Validator found", this.ValidatorId, validatorInstances.Count(),this.ValidatorId);

            return validatorInstances;
        }

        protected virtual void LoadValidatonInstances(List<ConfigSection> configValues)
        {

            try {

                if (this._validatorInstances == null)
                {
                    var validatorInstance = new ValidatorInstance(this.ValidatorId, this.Name, this.Description);
                    this._validatorInstances = new Dictionary<IValidatorInstance, ValidatorSettings>();

                    _validatorInstances.Add(validatorInstance, null);
                }
            }
            catch(Exception ex)
            {
                log.Error("An exception occurred loading validator instances for {ValidatorId}", this.ValidatorId);
                throw ex;
            }
        }
    }
}