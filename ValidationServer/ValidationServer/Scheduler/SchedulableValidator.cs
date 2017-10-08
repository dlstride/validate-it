using PrecisionDiscovery.Diagnostics;
using ValidationServer.Data.Scheduler;

namespace ValidationSchedulerData.Scheduler
{
    public class SchedulableValidator : ISchedulableValidator
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public SchedulableValidator(string validatorId, string validatorInstanceId, bool isScheduled = false, IScheduledValidation scheduledValidation = null )
        {
            this.ValidatorId = Guard.NotNullOrEmpty(validatorId, "validatorId", log);
            this.ValidatorInstanceId = Guard.NotNullOrEmpty(validatorInstanceId, "validatorInstanceId", log);

            this.IsScheduled = isScheduled;
            this.ScheduledValidation = scheduledValidation;
        }

        public bool IsScheduled { get; }

        public IScheduledValidation ScheduledValidation { get; }

        public string ValidatorInstanceId { get; }

        public string ValidatorId { get; }
    }
}