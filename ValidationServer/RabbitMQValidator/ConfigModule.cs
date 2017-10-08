using Ninject.Modules;
using RabbitMQValidator.Validators;
using ValidationServer.Data.Validators;

namespace RabbitMQValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<RabbitMQValidatorProxy>().InTransientScope();

            this.Kernel.Bind<RabbitMQErrorValidator>().ToSelf().InTransientScope();
        }
    }
}