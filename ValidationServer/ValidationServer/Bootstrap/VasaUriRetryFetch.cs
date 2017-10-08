using PrecisionDiscovery.Diagnostics;
using PrecisionDiscovery.Diagnostics.Logging;
using PrecisionDiscovery.Net.ServiceRequest;
using PrecisionDiscovery.Utility;
using System.Collections.Generic;

namespace PrecisionDiscovery.Configuration.Vasa
{
    /// <summary>
    /// Get Vasa sections, but retry several times.
    /// </summary>
    internal class VasaUriRetryFetch : IVasaFetch
    {
        /// <summary>
        /// log
        /// </summary>
        protected IPDLogger log = PDLogManager.GetCurrentClassLogger();

        private XMLServiceRequest _xsr = null;
        private string _baseURI = string.Empty;
        private string _configName = string.Empty;

        /// <summary>
        /// Create a new instance of VasaRetryFetch
        /// </summary>
        /// <param name="retryCount">how many times to retry</param>
        /// <param name="retryDelay">How long to wait between retries.</param>
        /// <param name="uri">Vasa Uri</param>
        public VasaUriRetryFetch(int retryCount, int retryDelay, string uri)
        {
            this.RetryCount = retryCount;
            this.RetryDelay = retryDelay;

            this._baseURI = Guard.NotNullOrEmpty(uri, uri, log);
        }

        /// <summary>
        /// How many times to retry
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// How long to wait between retries in millisecond.
        /// </summary>
        public int RetryDelay { get; set; }

        /// <summary>
        /// Get a list of config sections.
        /// </summary>
        /// <param name="credentials">User credentials</param>
        /// <param name="configName">Config name for this VasaClient</param>
        /// <param name="sectionName">Section name to get</param>
        /// <returns>List of sections if available.</returns>
        public List<ConfigSection> GetConfigSections(Authentication.IUserCredentials credentials, string configName, string sectionName)
        {
            this.ValidateConfigName(configName);

            if (string.IsNullOrEmpty(this._baseURI) || (this._xsr == null))
            {
                this.Init();
            }

            string url = string.Format("{0}/{1}/@@", this._baseURI, sectionName);

            List<ConfigSection> sections = Retry.Do<List<ConfigSection>>(this.RetryCount, this.RetryDelay, () =>
            {
                return this._xsr.SendRequest<List<ConfigSection>>(credentials, url, null, "GET");
            },
                ex => log.Fatal("Unable to get section {0} from Vasa at {1}\r\n{2}", sectionName, this._baseURI, ex),
                (count, incEx) => log.Fatal("Try {0} Unable to get section {1} from Vasa at {2}\r\n{3}", count, sectionName, this._baseURI, incEx));

            return sections;
        }

        private void Init()
        {
            // ConfigurationManager.AppSettings[this._configName];
            Guard.NotNullOrEmpty(this._baseURI, log, "Unable to find Vasa server URI in \"{0}\" configuration section", this._configName);

            this._xsr = new XMLServiceRequest(this._baseURI);
        }

        private void ValidateConfigName(string configName)
        {
            Guard.NotNullOrEmpty(configName, "configName", log);
            if (this._configName == string.Empty)
            {//lock in the name
                this._configName = configName;
            }
            Guard.IsTrue(configName == this._configName, log
                , "You can't use this fetch object with more than one Vasa client looks like this one is used by both {0} and {1}", this._configName, configName);
        }
    }
}