using Ninject;
using Ninject.Extensions.ChildKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Bootstrap;

namespace ValidationServerTests.TestHelpers
{
    public class TestErrorChildKernelCreator : IChildKernelCreator
    {
        //null guy for testing
        public ChildKernel CreateChildKernelWithModule(IKernel kernel, string module)
        {
            return null;
        }
    }
}
