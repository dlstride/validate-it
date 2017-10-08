using PrecisionDiscovery.Diagnostics.Logging;
using Topshelf;
using ValidationServer.Bootstrap;

namespace ValidationServer
{
    public class Program
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            //TODO: What if we do if Vasa is down? Send an email to dev not using notification? Contingency plan
            PDLogManager.Configure(config => { config.UseVasaForConfiguration("ValidationServer", "Logging"); });

            HostFactory.Run(host =>
            {
                host.Service<ValidationService>(ValidationService.Configuration);

                host.RunAsPrompt();
                host.StartAutomatically();
                host.EnableShutdown();
                host.SetServiceName("ValidationService");
                host.SetDisplayName("PD Validation Service");
                host.SetDescription("Runs PD scheduled validations.");
            });
        }
    }
}