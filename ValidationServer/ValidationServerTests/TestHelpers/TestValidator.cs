using System.Collections.Generic;
using ValidationServer.Data.Validators;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidator : Validator, IValidator
    {
        private TestValidatorSettings _settings;
        public TestValidator(TestValidatorSettings validatorSettings) : base(TestValidatorProxy.TestValidatorId)
        {
            _settings = validatorSettings;
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            return null;
        }
    }
}
