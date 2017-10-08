using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;
using Microsoft.Owin.Hosting;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using PrecisionDiscovery.Messaging;
using PrecisionDiscovery.Messaging.Configuration;
using PrecisionDiscovery.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Http;
using Topshelf;
using Topshelf.ServiceConfigurators;
using ValidationServer.Data.Scheduler;
using ValidationServer.Interfaces;

namespace ValidationServer.Bootstrap
{
    internal class ValidationService
    {
        private readonly static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        internal const string ServiceName = "ValidationService";
        internal static readonly string Identifier;
        internal static readonly string ServerUser;


        static ValidationService()
        {
            Identifier = string.Format("{0}:{1}", Environment.MachineName, ServiceName);
            ServerUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseNinjectMiddleware(ConfigModule.GetKernel);

            this.ConfigureWebApi(app);
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseNinjectWebApi(config);
        }

        public static void Configuration(ServiceConfigurator<ValidationService> config)
        {
            IKernel kernel = null;

            IDisposable webapiApp = null;

            config.ConstructUsing(() =>
            {
                return new ValidationService();
            });

            config.BeforeStartingService(h =>
            {
                h.RequestAdditionalTime(ServerSettings.AdditionalStartupTime);

                kernel = ConfigModule.GetKernel();

                log.Info("{0} Service Started", ServiceName);
            });

            config.WhenStarted(s =>
            {
                //configuration for hangfire server. 
                //tells it to use in memory storage instead of having to have a SQL backend
                Hangfire.GlobalConfiguration.Configuration.UseMemoryStorage();

                // tells hangfire which IoC to use when newing up jobs.
                Hangfire.GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

                RunValidationServer(kernel);

                webapiApp = WebApp.Start(ServerSettings.WebAPIUrl, builder =>
                {
                    kernel.Get<ValidationService>().Configuration(builder);
                });
            });

            config.AfterStartingService(() =>
            {
#if DEBUG

#endif
            });

            config.WhenStopped(s =>
            {
                if (kernel != null)
                {
                    var busCleanup = kernel.Get<IBusCleanup>();
                    busCleanup.StopAllBuses();

                    log.Info("{0} Service Stopped - bus cleanup and kernel disposed", ServiceName);
                }

                //In theory, IBusCleanup should allow notifications to be sent before an app closes down.
                //However, in this case more time was required.
                //TODO - verify not needed System.Threading.Thread.Sleep(500);

                log.Info("{0} Service Stopped", ServiceName);

                webapiApp.Dispose();

                kernel.Dispose();
            });

        }

        private static void ScheduleValidators(IKernel kernel, IValidationServer validationServer)
        {
            //TODO this goes away when UI ready
            var scheduleInputParser = kernel.Get<IScheduleInputParser>();
            IEnumerable<IScheduleRequest> scheduleRequests = scheduleInputParser.ParseAndCreateScheduleRequest();
            validationServer.AddValidationsToSchedule(scheduleRequests);
        }


        private static void RunValidationServer(IKernel kernel)
        {
            try
            {
                var validationServer = kernel.Get<IValidationServer>();

                //TODO this goes away when UI ready
                var scheduleInputParser = kernel.Get<IScheduleInputParser>();
                IEnumerable<IScheduleRequest> scheduleRequests = scheduleInputParser.ParseAndCreateScheduleRequest();
                validationServer.AddValidationsToSchedule(scheduleRequests);
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);
                SendErrorNotification(kernel, ex.Message);
                //Do not start service if configuration was incorrect.
                throw ex;
            }
        }
        private static void SendErrorNotification(IKernel kernel, string error)
        {
            var publishEndpoint = kernel.Get<IPublishEndpoint>();

            if (publishEndpoint == null)
                return;

            var notification = new NotificationMessage()
            {
                ApplicationId = "ValidationServer",
                NotificationTitle = string.Format("Validation Server Unhandled exception"),
                PhysicalSource = Environment.MachineName,
                UserId = WindowsIdentity.GetCurrent().Name,
                Severity = NotificationSeverity.Critical,
                Action = NotificationActions.Failed,
                Description = error
            } as INotificationMessage;

            publishEndpoint.Publish(notification);
        }
    }
}