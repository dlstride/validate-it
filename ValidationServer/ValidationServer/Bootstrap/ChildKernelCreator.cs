using Ninject;
using Ninject.Extensions.ChildKernel;
using PrecisionDiscovery.IO;
using System;

namespace ValidationServer.Bootstrap
{
    public class ChildKernelCreator : IChildKernelCreator
    {
        public const string ValidatorModuleNotFound = @"An error occurred configuring the ValidatorModules section in app.config: {0} does not exist";

        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ChildKernel CreateChildKernelWithModule(IKernel kernel, string module)
        {
            var childKernel = new ChildKernel(kernel);

            log.Debug("Attempting to load module {moduleName} in ChildKernelCreator", module);

            if (!PDFile.Exists(module))
            {
                throw new Exception(string.Format(ValidatorModuleNotFound, module));
            }

            childKernel.Load(module);

            return childKernel;
        }
    }
}
