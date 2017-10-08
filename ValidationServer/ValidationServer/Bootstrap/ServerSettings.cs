using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Linq;

namespace ValidationServer.Bootstrap
{
    public class ServerSettings
    {
        private readonly static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        static ServerSettings()
        {
            try
            {
                var vasa = new VasaClient("ValidationServer");
                var section = vasa.GetConfigSections(null, "Server").Single();

                var a = section["AdditionalStartupTime", false];
                var wu = section["WebAPIUrl"];

                AdditionalStartupTime = (string.IsNullOrWhiteSpace(a)) ? TimeSpan.FromSeconds(30) : TimeSpan.FromSeconds(Convert.ToInt32(a));
                WebAPIUrl = wu.Replace("{localhost}", Environment.MachineName);
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "There was a problem configuring the server settings");
                throw;
            }
        }

        public static TimeSpan AdditionalStartupTime { get; private set; }

        public static string WebAPIUrl { get; private set; }
    }
}