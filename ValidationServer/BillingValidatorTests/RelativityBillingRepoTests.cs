using BillingValidator.Repositories.RelativityReposs;
using System;
using Xunit;

namespace BillingValidatorTests
{
    public class RelativityBillingRepoTests
    {
        [Theory, AutoData_NSubstitute]
        public void RelativityBillingRepoInitWithNullParametersThrow()
        {
            Assert.ThrowsAny<Exception>(() => { RelativityBillingRepo bvp = new RelativityBillingRepo(null); });
        }
    }
}
