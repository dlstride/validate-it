using BillingValidator.Repositories.RelativityRepos;
using BillingValidator.Repositories.RelativityReposs;
using BillingValidator.Validators;
using Ninject;
using Ninject.Modules;
using PrecisionDiscovery.Configuration.Vasa;
using ValidationServer.Data.Validators;

namespace BillingValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IRelativityBillingRepo>().To<RelativityBillingRepo>().InTransientScope();

            this.Kernel.Bind<IRelativityProvidersRepo>().To<RelativityProvidersRepo>().InTransientScope();

            this.Kernel.Bind<BillingRanValidator>().ToSelf().InTransientScope();

            this.Kernel.Bind<IValidatorProxy>().To<BillingValidatorProxy>().InTransientScope();

            this.ConfigureVasaForChildKernel();  
        }
        //TODO: Verify same Vasas cross two validators projects dont conflict
        private void ConfigureVasaForChildKernel()
        {
            VasaModule vm = new VasaModule();

            vm.ConfigureClient("GPOD", gpodConfig =>
            {
                gpodConfig.BindingName = "GPODVasa";
                gpodConfig.TargetVariableName = "gpodVasa";
            });

            this.Kernel.Load(vm);
        }
    }
}