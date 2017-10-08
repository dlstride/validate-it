using PrecisionDiscovery.Diagnostics;
using System;
using ValidationServer.Scheduler.Scheduler;

namespace ValidationServer.Scheduler
{
    public static class JobIdUtils
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        private const char JobIdDelimeter = '_';

        public static string GetJobId(string validatorId, string validatorInstanceId)
        {
            Guard.NotNullOrEmpty(validatorId, "validatorId", log);
            Guard.NotNullOrEmpty(validatorInstanceId, "validatorInstanceId", log);

            return string.Format("{0}{1}{2}", validatorId, JobIdDelimeter, validatorInstanceId);
        }

        public static ValidatorInstanceInfo GetInstanceInfo(string jobId)
        {
            string[] delimitedJobId = jobId.Split(new char[] { JobIdDelimeter }, StringSplitOptions.RemoveEmptyEntries);

            if (delimitedJobId.Length != 2)
                throw new Exception(string.Format("Parsing callback JobId: {0} failed inside ValidatorSchedule", jobId));

            return new ValidatorInstanceInfo(delimitedJobId[0], delimitedJobId[1]); ;
        }
    }
}