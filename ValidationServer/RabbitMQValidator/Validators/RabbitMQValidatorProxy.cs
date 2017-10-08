using Ninject;
using PrecisionDiscovery.Configuration.Vasa;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using ValidationServer.Data.Validators;
using System.Linq;
using Ninject.Parameters;

namespace RabbitMQValidator.Validators
{
    public class RabbitMQValidatorProxy : ValidatorProxy, IValidatorProxy
    {
        public const string RabbitMQValidatorId = "RabbitMQValidator";

        public const string RabbitMQValidatorDescription = "RabbitMQ Validator: Make sure no messages in error queues";

        public const string RabbitMQValidatorName = "RabbitMQ Validator";

        private const string VasaInstanceItemName = "ValidatorInstanceSettings";

        public RabbitMQValidatorProxy(IKernel childKernel, IVasaClient vasa) : base(childKernel, RabbitMQValidatorId, RabbitMQValidatorName, RabbitMQValidatorDescription)
        {
        }

        protected override void LoadValidatonInstances(List<ConfigSection> configValues)
        {
            if (_validatorInstances == null)
            {
                _validatorInstances = new Dictionary<IValidatorInstance, ValidatorSettings>();

                List<RabbitMQValidatorSettings> vasaURIsAndSections = this.GetRabbitMQVasaURIsAndSections(configValues);

                foreach (var messagingVasainfo in vasaURIsAndSections)
                {
                    log.Debug("RabbitMQValidatorProxy validate instance for Server: {Server}, vHost: {vHost}", messagingVasainfo.HostServer, messagingVasainfo.vHost);

                    var serverString = messagingVasainfo.HostServer.Replace(".pd.local", "").Replace(".pddev.local", "");

                    var validatorInstanceId = string.Format("{0}.{1}.{2}", this.ValidatorId, serverString, messagingVasainfo.vHost);

                    var description = string.Format("{0} for Rabbit Server: {1}  vHost: {2}", this.Description, messagingVasainfo.HostServer, messagingVasainfo.vHost);
                    var vi = new ValidatorInstance(validatorInstanceId, messagingVasainfo.VasaURI, description);
                    this._validatorInstances.Add(vi, messagingVasainfo);
                }
            }

        }

        public IValidator Create(IValidatorInstance validatorInstance)
        {
            Guard.NotNull(validatorInstance, "validatorInstance", log);

            log.Info("RabbitMQValidatorProxy create validator instance for {Name}", validatorInstance.Name);

            if (this._validatorInstances == null)
            {
                throw new Exception(string.Format("ValidatorInstance Dictionary was null - GetAvailableValidators not called: {0}", validatorInstance.Name));
            }

            RabbitMQValidatorSettings settings = (RabbitMQValidatorSettings)this._validatorInstances[validatorInstance];

            if (settings == null)
            {
                throw new Exception(string.Format("RabbitMQ Validator Instance not found: {0}", validatorInstance.Name));
            }

            return this._childKernel.Get<RabbitMQErrorValidator>(new ConstructorArgument(RabbitMQErrorValidator.RabbitMQValidatorSettingsParamName, settings));
        }

        private List<RabbitMQValidatorSettings> GetRabbitMQVasaURIsAndSections(List<ConfigSection> configValues)
        {
            var infos = new List<RabbitMQValidatorSettings>();

            var validatorConfigSection = configValues.FirstOrDefault();
            if (validatorConfigSection != null)
            {
                var messagingSections = validatorConfigSection.Values.Where(s => s.Name == VasaInstanceItemName);

                log.Debug("RabbitMQValidatorProxy GetRabbitMQVasaURIsAndSections: found {RabbitValidatorCount} rabbitmq instances", messagingSections.Count());

                foreach (var section in messagingSections)
                {
                    string[] splitSection = section.Value.Split(new char[] { ',' });

                    if (splitSection.Length != 2)
                    {
                        log.Debug("RabbitMQValidatorProxy GetRabbitMQVasaURIsAndSections: VasaURI {VasaValue}", section.Value);

                        throw new Exception(string.Format("Vasa section {0} was incorrectly formatted! Expected format is 'VasaURI, SectionName'", section.Value));
                    }

                    var mvi = new RabbitMQValidatorSettings(splitSection[0].Trim(), splitSection[1].Trim());
                    infos.Add(mvi);

                    log.Debug("RabbitMQValidatorProxy GetRabbitMQVasaURIsAndSections: VasaURI {VasaURI} and SectionName: {SectionName}", mvi.VasaURI, mvi.SectionName);
                }
            }

            return infos;
        }
    }
}