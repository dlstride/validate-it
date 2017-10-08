using Ninject.Modules;
using ValidationServer.Data.Validators;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidatorModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<TestValidatorProxy>().InTransientScope();
        }
    }
}
