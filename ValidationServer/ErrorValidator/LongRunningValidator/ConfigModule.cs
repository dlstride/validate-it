using LongRunningValidator.Validators;
using Ninject.Modules;
using ValidationServer.Data.Validators;

namespace LongRunningValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<LongRunningValidatorProxy>().InTransientScope();

            this.Kernel.Bind<LongRunningConfigurableValidator>().ToSelf().InTransientScope();
        }
    }
}