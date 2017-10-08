using BillingValidator.Repositories.RelativityRepos;
using System;
using Xunit;

namespace BillingValidatorTests
{
    public class RelativityProvidersRepoTests
    {
        [Theory, AutoData_NSubstitute]
        public void RelativityProvidersRepoInitWithNullParametersThrow()
        {
            Assert.ThrowsAny<Exception>(() => { RelativityProvidersRepo repo = new RelativityProvidersRepo(null); });
        }
    }
}
