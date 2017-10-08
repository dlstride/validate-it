using PrecisionDiscovery.Diagnostics;

namespace ValidationSchedulerData
{
    public class ScheduleRequest : IScheduleRequest
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public string ValidatorId { get; }

        public string ValidatorInstanceId { get; }

        public string CronTabExpression { get; }

        public ScheduleRequest(string validatorId, string validatorInstanceId, string cronTabExpression)
        {
            this.ValidatorId = Guard.NotNullOrEmpty(validatorId, "validatorId", log);
            this.ValidatorInstanceId = Guard.NotNullOrEmpty(validatorInstanceId, "validatorInstanceId", log);
            this.CronTabExpression = Guard.NotNullOrEmpty(cronTabExpression, "cronTabExpression", log);
        }
    }
}