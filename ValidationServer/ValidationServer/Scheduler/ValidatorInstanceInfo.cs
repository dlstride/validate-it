using PrecisionDiscovery.Diagnostics;

namespace ValidationServer.Scheduler.Scheduler
{
    public class ValidatorInstanceInfo
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ValidatorInstanceInfo(string validatorId, string validatorInstanceId)
        {
            this.ValidatorId = Guard.NotNull(validatorId, "validatorId", log);
            this.ValidatorInstanceId = Guard.NotNull(validatorInstanceId, "validatorInstanceId", log);
        }

        public string ValidatorId { get; }
        public string ValidatorInstanceId { get; set; }
    }    
}