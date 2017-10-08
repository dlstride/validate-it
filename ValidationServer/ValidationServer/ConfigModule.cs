using Hangfire;
using Ninject;
using Ninject.Modules;
using PrecisionDiscovery.Configuration.Vasa;
using PrecisionDiscovery.Configuration.Vasa.ConfigFile;
using PrecisionDiscovery.Container;
using PrecisionDiscovery.Diagnostics.POST;
using PrecisionDiscovery.Messaging.Configuration;
using System.Linq;
using ValidationSchedulerData.Scheduler;
using ValidationServer.Bootstrap;
using ValidationServer.Data.Scheduler;
using ValidationServer.Interfaces;
using ValidationServer.Notification;
using ValidationServer.Scheduler;

namespace ValidationServer
{
    public class ConfigModule : NinjectModule
    {
        private static IKernel _kernel = null;
        public static IKernel GetKernel()
        {
            if (ConfigModule._kernel == null)
            {
                ConfigModule._kernel = ConfigModule.ConfigureNinject();
            }
            return ConfigModule._kernel;
        }

        internal const string PrimaryBus = "PrimaryBus";
        private static IKernel ConfigureNinject()
        {
            var kernel = NinjectExtensions.ConfigureNinject(config =>
            {
                config.ConfigureWithVasa()
                   .SetDefaultCacheStrategy(() => { return new VasaSimpleCache(10 * 60); })
                   .SetDefaultFetchStrategy(() => { return new VasaConfigFetch(new VasaRetryFetch(10, 1000)); })

                   .ConfigureDefaultClient(vconfig =>
                   {
                       vconfig.ThrowIfUndeclaredSectionIsAccessed = true;
                   })

                   .ConfigureClient("ValidationServer", vpConfig =>
                   {
                       vpConfig.BindingName = "ValidationServerVasa";
                       vpConfig.TargetVariableName = "vsVasa";
                   })
                   ;

                config.ConfigureWithMassTransit()
                  .AddBus(ConfigModule.PrimaryBus, "ValidationServerVasa", "Validation.Messaging", busCfg =>
                  {
                      busCfg.TargetVariableName = "pBus";
                  });

                config.ConfigureWith<ConfigModule>();
            });

            return kernel;

        }

        public override void Load()
        {
            this.Bind<IValidatorModuleConfig>().To<ValidatorModuleConfig>().InTransientScope();

            this.Bind<IChildKernelCreator>().To<ChildKernelCreator>().InTransientScope();

            this.Bind<BackgroundJobServer>().ToSelf().InSingletonScope();

            this.Bind<HangFireScheduleCallback>().ToSelf().InSingletonScope();

            this.Bind<IScheduleCallback>().ToMethod(c => c.Kernel.Get<HangFireScheduleCallback>());

            this.Bind<HangFireScheduler>().ToSelf().InSingletonScope();

            this.Bind<IValidationScheduler>().ToMethod(c => c.Kernel.Get<HangFireScheduler>());

            this.Bind<IPOST>().To<VSPost>().InTransientScope();

            this.Bind<IScheduleRequest>().To<ScheduleRequest>().InTransientScope();

            this.Bind<IScheduleInputParser>().To<ScheduleInputParser>().InTransientScope();

            this.Bind<IValidatorProvider>().To<ValidatorProvider>().InSingletonScope();

            this.Bind<IValidatorRunner>().To<ValidatorRunner>().InSingletonScope();

            this.Bind<IValidationServer>().To<ValidationServer>().InSingletonScope();

            this.BindValidationResultHandlers();
        }

        private void BindValidationResultHandlers()
        {
            if (this.EnableNotificationResultHandler())
            {
                this.Bind<IValidationResultHandler>().To<NotificationValidationResultHandler>().InTransientScope();
            }

            this.Bind<IValidationResultHandler>().To<ConsoleNotificationHandler>().InTransientScope();

            this.Bind<IValidationResultHandler>().To<LogValidationResultHandler>().InTransientScope();         
        }

        private bool EnableNotificationResultHandler()
        {
            IVasaClient vs = this.Kernel.Get<IVasaClient>("ValidationServerVasa");
            var disable = vs.GetConfigSections(null, "Server").SingleOrDefault()["DisableResultNotifications", false];
            return string.Compare(disable, "true", true) != 0;
        }
    }
}