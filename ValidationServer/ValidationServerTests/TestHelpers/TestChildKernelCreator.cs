using Ninject;
using Ninject.Extensions.ChildKernel;
using ValidationServer.Bootstrap;

namespace ValidationServerTests.TestHelpers
{
    public class TestChildKernelCreator : IChildKernelCreator
    {
        //Parameter module is not used here because we are creating a fake one instead.
        public ChildKernel CreateChildKernelWithModule(IKernel kernel, string module)
        {
            var testModule = new TestValidatorModule();

            var childKernel = new ChildKernel(kernel);

            childKernel.Load(testModule);

            return childKernel;
        }
    }
}
