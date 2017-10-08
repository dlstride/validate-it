using System.Collections.Generic;
using System.Linq;
using ValidationServer.Data.Validators;
using ValidationServer.Interfaces;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidatorProvider : IValidatorProvider
    {
        public IEnumerable<IValidatorProxy> ValidatorProxies
        {
            get
            {
                return new List<IValidatorProxy>();
            }
        }

        public IValidatorInstance GetValidatorInstance(string validationId, string validatorInstanceId)
        {
            var validatorProxy = ValidatorProxies.Where(proxy => proxy.ValidatorId == validationId).SingleOrDefault();
            var validatorInstance = validatorProxy.ValidatorInstances.Where(instance => instance.ValidatorInstanceId == validatorInstanceId).SingleOrDefault();
            return validatorInstance;
        }

        public IValidatorProxy GetValidatorProxy(string validationId)
        {
            var validatorProxy = ValidatorProxies.Where(proxy => proxy.ValidatorId == validationId).SingleOrDefault();
            return validatorProxy;
        }
    }
}
