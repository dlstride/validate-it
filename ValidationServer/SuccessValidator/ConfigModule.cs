using Ninject.Modules;
using SuccessValidator.Validators;
using ValidationServer.Data.Validators;

namespace SuccessValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<SuccessValidatorProxy>().InTransientScope();

            this.Kernel.Bind<ResultAlwaysSuccessValidator>().ToSelf().InTransientScope();
        }
    }
}
