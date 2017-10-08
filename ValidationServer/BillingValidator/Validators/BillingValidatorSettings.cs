using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationServer.Data.Validators;

namespace BillingValidator.Validators
{
    public class BillingValidatorSettings : ValidatorSettings
    {
        public const string BillingRunTimeVasaItemName = "BillingRunTime";

        public BillingValidatorSettings(List<ConfigSection> validatorConfigValues)
        {
            this.LoadVasaValues(validatorConfigValues);
        }

        //If current time before BillingRunTime then use yesterday's date
        //  Example: 8am then check yesterday's billing values
        public int? BillingRunTime { get; set; }

        private void LoadVasaValues(List<ConfigSection> validatorConfigValues)
        {
            log.Debug("BillingValidator: Loading Vasa values pertinent to the BillingValidator");

            this.LoadBillingRunningTime(validatorConfigValues);
        }

        private void LoadBillingRunningTime(List<ConfigSection> validatorConfigValues)
        {
            log.Debug("BillingValidator: Loading Billing run time.");

            int? dateToCheck = null;

            var validatorConfigSection = validatorConfigValues.FirstOrDefault();
            if (validatorConfigSection != null)
            {
                var billingRunTime = validatorConfigSection[BillingRunTimeVasaItemName, false];
                if (!string.IsNullOrEmpty(billingRunTime))
                {
                    int runTime = 0;
                    if (Int32.TryParse(billingRunTime, out runTime))
                    {
                        dateToCheck = runTime;
                    }
                }
            }

            this.BillingRunTime =  dateToCheck;
        }
    }
}