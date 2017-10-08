using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using ValidationServer.Data.Validators;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidatorProxy : IValidatorProxy
    {
        public static string TestValidatorId = "PD.Test.Validator";

        private List<IValidatorInstance> _validatorInstances { get; set; }

        public IEnumerable<IValidatorInstance> ValidatorInstances
        {
            get
            {
                if (_validatorInstances == null)
                {
                    _validatorInstances = new List<IValidatorInstance>() { new TestValidatorInstance() };
                }

                return _validatorInstances;
            }

        }

        public string Description
        {
            get
            {
                return "To be used for unit-testing";
            }
        }

        public string Name
        {
            get
            {
                return "TestValidator";
            }
        }

        public string ValidatorId
        {
            get
            {
                return TestValidatorId;
            }
        }

        public Version Version
        {
            get
            {
                return new Version("1.0");
            }
        }

        public IEnumerable<IValidatorInstance> GetAvailableValidators(List<ConfigSection> configValues)
        {
            List<IValidatorInstance> instances = new List<IValidatorInstance>();

            instances.Add(new TestValidatorInstance());

            return instances;
        }

        private void LoadValidatonInstances(List<ConfigSection> configValues)
        {
            List<IValidatorInstance> instances = new List<IValidatorInstance>();

            instances.Add(new TestValidatorInstance());

            _validatorInstances.AddRange(instances);
        }

        public IValidator Create(IValidatorInstance validationInstance)
        {
            TestValidatorSettings testValidatorSettings = new TestValidatorSettings();

            return new TestValidator(testValidatorSettings);
        }       
    }
}
