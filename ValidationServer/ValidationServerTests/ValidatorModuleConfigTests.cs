using System;
using System.Collections.Specialized;
using ValidationServer.Bootstrap;
using Xunit;

namespace ValidationServer.Tests
{
    public class ProgramTests
    {
        //[Theory, AutoData_NSubstitute]
        //public void GetValidatorsInConfigWithNullAndNonNullParameters(NameValueCollection moduleConfigs)
        //{
        //    Assert.NotNull(ValidatorModuleConfig.GetValidatorsInConfig(moduleConfigs));
        //    Assert.ThrowsAny<Exception>(() => ValidatorModuleConfig.GetValidatorsInConfig(null));
        //}

        //[Theory, AutoData_NSubstitute]
        //public void GetValidatorsInConfigWithNullValueThrows(NameValueCollection moduleConfigs, string testKey)
        //{
        //    moduleConfigs.Add(testKey, null);
        //    Assert.ThrowsAny<Exception>(() => { ValidatorModuleConfig.GetValidatorsInConfig(moduleConfigs); });
        //}

        //[Theory, AutoData_NSubstitute]
        //public void AddKeyToNameValueShouldAppearInModules(NameValueCollection moduleConfigs, string testKey, string testValue)
        //{
        //    moduleConfigs.Add(testKey, testValue);
        //    Assert.Contains(testValue, ValidatorModuleConfig.GetValidatorsInConfig(moduleConfigs));
        //}
    }
}