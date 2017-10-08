using BillingValidator.Validators;
using System;
using Xunit;

namespace BillingValidatorTests
{
    public class BillingValidatorProxyTests
    {
        [Theory, AutoData_NSubstitute]
        public void BillingValidatorProxyInitWithNullParametersThrow()
        {
            Assert.ThrowsAny<Exception>(() => { BillingValidatorProxy bvp = new BillingValidatorProxy(null, null); });
        }
    }
}
