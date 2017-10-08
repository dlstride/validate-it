using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace ValidationServer.Bootstrap
{
    public class ValidatorModuleConfig : IValidatorModuleConfig
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public List<string> ModuleNames { get; set; }

        public ValidatorModuleConfig()
        {
            this.GetConfiguredValidatorModules();
        }

        private void GetConfiguredValidatorModules()
        {
            NameValueCollection moduleConfigs = this.GetModuleListFromAppConfig();
            this.ModuleNames = this.GetValidatorsInConfig(moduleConfigs);
        }

        private NameValueCollection GetModuleListFromAppConfig()
        {
            log.Debug("Getting Validation Modules from Config file");

            return ConfigurationManager.GetSection("ValidatorModules") as NameValueCollection;
        }

        private List<string> GetValidatorsInConfig(NameValueCollection moduleConfigs)
        {
            List<string> modules = new List<string>();

            if (moduleConfigs == null)
            {
                log.Error("There were no ValidatorModules defined: Most likely the configSection for validator modules missing");
                throw new Exception("There was no ValidatorModules section found in app config!");
            }

            log.Debug("{ConfigCount} Key/Value pairs found in the config file.", moduleConfigs.Keys);

            foreach (string key in moduleConfigs.Keys)
            {
                string moduleAssemblyName = Guard.NotNullOrEmpty(moduleConfigs[key], "moduleAssemblyName", log);
                log.Debug("Module with Name: {moduleAssemblyName} found in the configuration.", moduleAssemblyName);
                modules.Add(moduleAssemblyName);
            }

            return modules;
        }
    }
}