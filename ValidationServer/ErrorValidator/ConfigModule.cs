using ErrorValidator.Validators;
using Ninject.Modules;
using ValidationServer.Data.Validators;

namespace ErrorValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<ErrorValidatorProxy>().InTransientScope();

            this.Kernel.Bind<ResultAlwaysErrorValidator>().ToSelf().InTransientScope();
        }
    }
}