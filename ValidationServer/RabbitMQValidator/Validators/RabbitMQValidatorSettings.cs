using PrecisionDiscovery.Configuration.Vasa;
using System.Linq;
using ValidationServer.Data.Validators;

namespace RabbitMQValidator.Validators
{
    public class RabbitMQValidatorSettings : ValidatorSettings
    {
        public RabbitMQValidatorSettings(string vasaURI, string sectionName)
        {
            this.VasaURI = vasaURI;
            this.SectionName = sectionName;

            this.LoadSettings(vasaURI, sectionName);
        }

        private void LoadSettings(string vasaURI, string sectionName)
        {
            LoadConnectionSettings(vasaURI, sectionName);
        }

        private void LoadConnectionSettings(string vasaURI, string sectionName)
        {
            var vasa = new VasaClient(vasaURI);

            var common = vasa.GetConfigSections(null, sectionName).Single();
            if (common["RabbitServer", false] != null)
            {
                this.GetOldRabbitSettings(common);
            }
            else
            {
                this.GetNewRabbitSettings(common);
            }
        }

        public string VasaURI { get; set; }

        public string SectionName { get; set; }

        public string HostServer { get; set; }

        public string Port { get; set; }

        public string vHost {get; set;}

        public string UserName { get; set; }

        public string Password { get; set; }

        private void  GetOldRabbitSettings(ConfigSection messagingSection)
        {
           this.HostServer = messagingSection["RabbitServer"];
            this.Port = messagingSection["RabbitPort"];
            this.vHost = messagingSection["vHost"];
            this.UserName = messagingSection["RabbitUser"];
            this.Password = messagingSection["RabbitPass"];            
        }

        private  void GetNewRabbitSettings(ConfigSection messagingSection)
        {
            this.HostServer = messagingSection["HostAddress"];
            this.Port = messagingSection["HostPort"];
            this.vHost = messagingSection["vHost"];

            this.UserName = messagingSection["HostUsername"];
            this.Password = messagingSection["HostPassword"];           
        }
    }
}