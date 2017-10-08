using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer;
using ValidationServer.Bootstrap;
using ValidationServer.Data.Validators;
using ValidationServer.Interfaces;
using ValidationServerTests.TestHelpers;
using Xunit;

namespace ValidationServerTests
{
    public class ValidatorProviderTests
    {
        [Theory, AutoData_NSubstitute]
        public void GetValidatorInstanceFailsOnNullParam(string validatorId, string validatorInstanceId)
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorInstance(validatorId, null);
            });

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorInstance(null, validatorInstanceId);
            });
        }

        [Theory, AutoData_NSubstitute]
        public void GetValidatorInstanceFailsOnNullProxyOrInstance(string validatorId, string validatorInstanceId, IValidatorProxy proxy)
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorInstance(validatorId, null);
            });

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorInstance(null, validatorInstanceId);
            });
        }

        [Theory, AutoData_NSubstitute]
        public void GetValidatorProviderFailsOnNullProxy()
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorProxy(null);
            });
        }

        [Theory, AutoData_NSubstitute]
        public void GetValidatorInstanceFailsOnNonDiscoveredInstance(string validatorInstanceId)
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorInstance(TestValidatorProxy.TestValidatorId, validatorInstanceId);
            });
        }

        [Theory, AutoData_NSubstitute]
        public void GetValidatorInstanceSucceedsOnDiscoveredInstance()
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            provider.GetValidatorInstance(TestValidatorProxy.TestValidatorId, TestValidatorInstance.TestValidatorInstanceId);
        }


        [Theory, AutoData_NSubstitute]
        public void DiscoveryFailsWhenChildKernelFails()
        {
            var kernel = GetKernelWithErrorBindings();
            Assert.ThrowsAny<Exception>(() =>
            {
                var provider = kernel.Get<IValidatorProvider>();
            });
        }

        [Theory, AutoData_NSubstitute]
        public void GetValidatorProviderFailsOnNotFoundProxy(string validatorId)
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            Assert.ThrowsAny<Exception>(() =>
            {
                provider.GetValidatorProxy(validatorId);
            });
        }

        [Theory, AutoData_NSubstitute]
        public void GetValidatorProviderSucceedsWhenFound(string validatorId, [Frozen] IEnumerable<IValidatorProxy> proxies)
        {
            var kernel = GetKernelWithBindings();
            var provider = kernel.Get<IValidatorProvider>();

            provider.GetValidatorProxy(TestValidatorProxy.TestValidatorId);
        }

        private IKernel GetKernelWithBindings()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IVasaClient>().ToMethod(config =>
            {
                return Substitute.For<IVasaClient>();
            }).InSingletonScope();

            kernel.Bind<IValidatorModuleConfig>().To<TestValidatorModuleConfig>().InSingletonScope();

            kernel.Bind<IChildKernelCreator>().To<TestChildKernelCreator>().InSingletonScope();

            kernel.Bind<IValidatorProvider>().To<ValidatorProvider>().InSingletonScope();

            return kernel;
        }

        private IKernel GetKernelWithErrorBindings()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IVasaClient>().ToMethod(config =>
            {
                return Substitute.For<IVasaClient>();
            }).InSingletonScope();

            kernel.Bind<IValidatorModuleConfig>().To<TestValidatorModuleConfig>().InSingletonScope();

            kernel.Bind<IChildKernelCreator>().To<TestErrorChildKernelCreator>().InSingletonScope();

            kernel.Bind<IValidatorProvider>().To<ValidatorProvider>().InSingletonScope();

            return kernel;
        }



    }
}
