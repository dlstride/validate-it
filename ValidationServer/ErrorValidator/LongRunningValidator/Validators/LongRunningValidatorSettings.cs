using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationServer.Data.Validators;

namespace LongRunningValidator.Validators
{
    public class LongRunningValidatorSettings : ValidatorSettings
    {
        public const int RunTimeInMinutesDefault = 60;

        public const string RunTimeInMinutesVasaItemName = "RunTimeInMinutes";

        public LongRunningValidatorSettings(List<ConfigSection> validatorConfigValues)
        {
            this.LoadVasaValues(validatorConfigValues);
        }

        public int? RunTimeInMinutes { get; set; }

        private void LoadVasaValues(List<ConfigSection> validatorConfigValues)
        {
            log.Debug("LongRunningValidator: Loading Vasa values pertinent to the LongRunningValidator");

            this.LoadRunningTime(validatorConfigValues);
        }

        private void LoadRunningTime(List<ConfigSection> validatorConfigValues)
        {
            log.Debug("LongRunningValidator: Loading LongRunningValidator run time.");

            int? runTimeVasa = null;

            var validatorConfigSection = validatorConfigValues.FirstOrDefault();
            if (validatorConfigSection != null)
            {
                var runTime = validatorConfigSection[RunTimeInMinutesVasaItemName, false];

                try
                {
                    runTimeVasa = Convert.ToInt32(runTime);
                }
                catch { }

                runTimeVasa = Convert.ToInt32(runTime);
            }

            this.RunTimeInMinutes = runTimeVasa ?? RunTimeInMinutesDefault;
        }
    }
}