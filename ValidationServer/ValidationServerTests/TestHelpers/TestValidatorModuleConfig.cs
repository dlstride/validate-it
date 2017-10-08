using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Bootstrap;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidatorModuleConfig : IValidatorModuleConfig
    {
        public List<string> ModuleNames
        {
            get
            {
                var x = Substitute.For<List<string>>();
                x.Add(TestValidatorProxy.TestValidatorId);

                x = new List<string>() { TestValidatorProxy.TestValidatorId };
                return x;
            }
        }
    }
}
