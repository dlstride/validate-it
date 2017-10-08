using Ninject.Modules;
using ValidationServer.Data.Validators;
using WarningValidator.Validators;

namespace WarningValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<WarningValidatorProxy>().InTransientScope();

            this.Kernel.Bind<ResultAlwaysWarningValidator>().ToSelf().InTransientScope();
        }
    }
}