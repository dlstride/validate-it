using Hangfire;
using PrecisionDiscovery.Diagnostics;
using System;
using ValidationServer.Scheduler.Scheduler;

namespace ValidationServer.Scheduler
{
    public delegate void CallBackToValidationServer(object sender, ValidatorInfoArgs e);

    public class HangFireScheduleCallback : IScheduleCallback
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public event CallBackToValidationServer RunValidatorEvent;

        public HangFireScheduleCallback()
        {
        }

        public void ScheduleValidator(string jobId, string cronTabExpression)
        {
            log.Info("HangFireScheduleCallback ScheduleValidator: scheduling job for {JobId} and cronTabExpression{CronTabExpression}", jobId, cronTabExpression);
            RecurringJob.AddOrUpdate(jobId, () => ScheduleCallBack(jobId), cronTabExpression);
        }

        public void ScheduleCallBack(string jobId)
        {
            Guard.NotNull(jobId, "jobId", log);

            ValidatorInstanceInfo validatorInfo = JobIdUtils.GetInstanceInfo(jobId);
            var validatorInfoArgs = new ValidatorInfoArgs(validatorInfo);

            if (RunValidatorEvent != null)
            {
                log.Debug("HangFireScheduleCallback ScheduleCallBack: schedule call back for job id {JobId}", jobId);
                RunValidatorEvent(this, validatorInfoArgs);
            }

#if DEBUG
            HangFireScheduler.JobsScheduledCalledBack++;
#endif
        }
    }

    public class ValidatorInfoArgs : EventArgs
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ValidatorInfoArgs(ValidatorInstanceInfo info)
        {
            Guard.NotNull(info, "info", log);
            this.ValidatorInfo = info;
        }

        public ValidatorInstanceInfo ValidatorInfo { get; }
    }
}