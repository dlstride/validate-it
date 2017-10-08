using Ninject;
using Ninject.Extensions.ChildKernel;

namespace ValidationServer.Bootstrap
{
    public interface IChildKernelCreator
    {
        ChildKernel CreateChildKernelWithModule(IKernel kernel, string module);
    }
}
