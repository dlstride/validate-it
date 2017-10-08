using PrecisionDiscovery.Diagnostics;
using PrecisionDiscovery.Utility.Eventing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;


namespace LongRunningValidator.Validators
{
    public class LongRunningConfigurableValidator : Validator, IValidator
    {
        public const string LongRunningSettingsConstructorParameterName = "longRunningSettings";

        private LongRunningValidatorSettings _settings;

        public LongRunningConfigurableValidator(LongRunningValidatorSettings longRunningSettings) :
            base(LongRunningValidatorProxy.LongRunningValidatorId)
        {
            this._settings = Guard.NotNull(longRunningSettings, "settings", log);
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Info("LongRunningValidator Execute started");

            List<IValidatorResult> validatorResults = new List<IValidatorResult>();

            string message = string.Format("Long Running validator is configured to run for {0} minutes and return result code of success.", _settings.RunTimeInMinutes);
            var vr = new ValidatorResult(this.ValidatorProxyId, message, ValidatorResultCode.Error, "LongRunningValidator");
            validatorResults.Add(vr);

            log.Info("Starting LongRunningValidator Execute completed with {ResultCount} results", validatorResults.Count);

            return validatorResults;
        }

        async Task LongRunningOperation()
        {
            await Task.Delay(TimeSpan.FromMinutes(this._settings.RunTimeInMinutes));
        }

        private async void SomeEvent(object sender, EventArgs e)
        {
            await LongRunningOperation();
        }
    }
}