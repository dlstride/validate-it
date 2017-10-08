using Hangfire;
using Hangfire.Storage;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ValidationSchedulerData.Scheduler;
using ValidationServer.Data.Scheduler;
using ValidationServer.Interfaces;

namespace ValidationServer.Scheduler
{
    public class HangFireScheduler : IValidationScheduler, IDisposable
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();
        private BackgroundJobServer _hangfireValidationServer;

#if DEBUG
        public static Stopwatch _stopWatch = new Stopwatch();
        public static int JobsScheduled = 0;
        public static int JobsScheduledCalledBack = 0;
#endif
        private IScheduleCallback _sheduleCallbacker;

        public HangFireScheduler(BackgroundJobServer backGroundJobServer, IScheduleCallback scheduleCallbacker)
        {
            this._hangfireValidationServer = Guard.NotNull(backGroundJobServer, "backGroundJobServer", log);
            this._sheduleCallbacker = Guard.NotNull(scheduleCallbacker, "scheduleCallbacker", log);
        }

        public IScheduledValidation AddValidationToSchedule(IScheduleRequest scheduleRequest)
        {
            Guard.NotNull(scheduleRequest, "scheduleRequest", log);

            log.Debug("HangFireScheduler AddValidationToSchedule for validationId {ValidatorId} validationInstanceId {ValidatorInstanceId} cronTabExpressions {CronTabExpression}",
                                scheduleRequest.ValidatorId, scheduleRequest.ValidatorInstanceId, scheduleRequest.CronTabExpression);

            IScheduledValidation scheduledValidation = this.AddOrEditValidationSchedule(scheduleRequest);

            return scheduledValidation;
        }

        public IEnumerable<IScheduledValidation> AddValidationsToSchedule(IEnumerable<IScheduleRequest> scheduleRequests)
        {
            Guard.NotNull(scheduleRequests, "scheduleRequests", log);
            this.ValidateScheduledRequestsInputNotNull(scheduleRequests);

            //TODO when UI ready need to figure out pattern for errors. Add all properly formated scheduleRequests. Return a list of the failed and successful validations to show user.
            //Right now the first invalid validation will throw an error.

            log.Info("HangFireScheduler AddValidationsToSchedule starting for {ScheduleCount}", scheduleRequests.Count());

            List<IScheduledValidation> results = new List<IScheduledValidation>();
            foreach (IScheduleRequest scheduleRequest in scheduleRequests)
            {

                IScheduledValidation scheduledValidation = this.AddValidationToSchedule( scheduleRequest);

                results.Add(scheduledValidation);
#if DEBUG
                JobsScheduled++;
#endif
            }

#if DEBUG
            //for (int i = 0; i < 50; i++)
            //{
            //    foreach (IScheduleRequest scheduleRequest in scheduleRequests)
            //    {
            //        var jobId = this.ValidateAndGetJobId(scheduleRequest.ValidatorId, scheduleRequest.ValidatorInstanceId);
            //        RecurringJob.Trigger(jobId);
            //    }
            //}
#endif

            return results;
        }

        public IScheduledValidation EditValidationSchedule(IScheduleRequest scheduleRequest)
        {
            Guard.NotNull(scheduleRequest, "scheduleRequest", log);
            this.ValidateScheduledRequestInputNotNull(scheduleRequest);

            log.Info("HangFireScheduler EditValidationSchedule for validationId {ValidatorId} validationInstanceId {ValidatorInstanceId} cronTabExpressions {CronTabExpression}",
                    scheduleRequest.ValidatorId, scheduleRequest.ValidatorInstanceId, scheduleRequest.CronTabExpression);

            if (!this.IsValidationScheduled(scheduleRequest.ValidatorId, scheduleRequest.ValidatorInstanceId))
            {
                throw new Exception("Validator's scheduled not found.");
            }

            return this.AddOrEditValidationSchedule(scheduleRequest);
        }

        public IEnumerable<IScheduledValidation> GetScheduledValidations()
        {
            log.Info("HangFireScheduler GetScheduledValidations");

            var scheduledValidations = this.GetRecurringJobs();

            log.Debug("HangFireScheduler GetScheduledValidations - {ScheduledJobs} found", scheduledValidations == null ? 0 : scheduledValidations.Count);

            return scheduledValidations;
        }

        public IEnumerable<IScheduledValidation> GetScheduledValidations(int start, int end)
        {
            log.Info("HangFireScheduler GetScheduledValidations starting at {Start} and ending at {Ending}", start, end);

            var scheduledValidations = this.GetRecurringJobs(start, end);

            log.Debug("HangFireScheduler GetScheduledValidations - {ScheduledJobs} found", scheduledValidations == null ? 0 : scheduledValidations.Count);

            return scheduledValidations;
        }

        public bool IsValidationScheduled(string validatorId, string validatorInstanceId)
        {
            log.Info("HangFireScheduler IsValidationScheduled starting for validationId {ValidationId} and validatorInstanceId {ValidatorInstanceId}", validatorId, validatorInstanceId);

            IEnumerable<IScheduledValidation> recurringJobs = this.GetRecurringJobs();
            var jobId = JobIdUtils.GetJobId(validatorId, validatorInstanceId);
            var recurringJob = recurringJobs.Where(job => job.JobId == jobId).SingleOrDefault();
            bool scheduled = recurringJob != null;

            log.Debug("HangFireScheduler IsValidationScheduled for JobId {JobId}: {IsScheduled}" , jobId, scheduled);

            return scheduled;
        }

        public void RemoveValidationFromSchedule(string validatorId, string validatorInstanceId)
        {
            string jobId = this.ValidateAndGetJobId(validatorId, validatorInstanceId);

            log.Info("HangFireScheduler RemoveValidationFromSchedule for validationId {ValidationId} and validatorInstanceId {ValidatorInstanceId}", validatorId, validatorInstanceId);

            RecurringJob.RemoveIfExists(jobId);
        }

        public void TriggerJob(string validatorId, string validatorInstanceId)
        {
            string jobId = this.ValidateAndGetJobId(validatorId, validatorInstanceId);
           
            if (!this.IsValidationScheduled(validatorId, validatorInstanceId))
            {
                //TODO This will change once UI is ready. Ability to trigger a validator even if it is not scheduled.
                throw new Exception( string.Format("Validator is not scheduled so it can not be trigerred: validatorId {0} validatorInstanceId {1}", validatorId, validatorInstanceId ));
            }

            log.Info("HangFireScheduler TriggerJob for validationId {ValidationId} and validatorInstanceId {ValidatorInstanceId}", validatorId, validatorInstanceId);

            RecurringJob.Trigger(jobId);
        }

        private IScheduledValidation AddOrEditValidationSchedule(IScheduleRequest scheduleRequest)
        {
            string jobId = JobIdUtils.GetJobId(scheduleRequest.ValidatorId, scheduleRequest.ValidatorInstanceId);

            var cronTabExpression = scheduleRequest.CronTabExpression;

            this._sheduleCallbacker.ScheduleValidator(jobId, cronTabExpression);

            var scheduledValidation = new ScheduledValidation(jobId, cronTabExpression, DateTime.Now) { };

            return scheduledValidation as IScheduledValidation;
        }

        private IList<IScheduledValidation> GetRecurringJobs()
        {
            log.Debug("HangFireScheduler GetRecurringJobs started");

            var storageConnection = this.GetJobStorageConnection();

            List<RecurringJobDto> recurringJobs = storageConnection.GetRecurringJobs();

            var count = recurringJobs == null ? 0 : recurringJobs.Count();

            log.Debug("HangFireScheduler GetRecurringJobs: found {JobCount} scheduled job(s)", recurringJobs);

            return this.GetScheduledValidationsFromRecurringJobs(recurringJobs);
        }

        private IList<IScheduledValidation> GetRecurringJobs(int startAt, int endingAt)
        {
            var storageConnection = this.GetJobStorageConnection();

            List<RecurringJobDto> recurringJobs = storageConnection.GetRecurringJobs(startAt, endingAt);

            return this.GetScheduledValidationsFromRecurringJobs(recurringJobs);
        }

        private JobStorageConnection GetJobStorageConnection()
        {
            JobStorageConnection storageConnection = null;

            using (IStorageConnection connection = JobStorage.Current.GetConnection())
            {
                storageConnection = connection as JobStorageConnection;

                if (storageConnection == null)
                {
                    throw new Exception("An error occurred getting the HangFire JobStorageConnection");
                }

            }
            return storageConnection;
        }

        private IList<IScheduledValidation> GetScheduledValidationsFromRecurringJobs(List<RecurringJobDto> recurringJobs)
        {
            IList<IScheduledValidation> scheduledValidations
                = recurringJobs.Select(x =>
                     new ScheduledValidation(x.Id, x.Cron, x.CreatedAt, x.LastExecution, x.NextExecution) { } as IScheduledValidation).ToList();

            return scheduledValidations.ToList();
        }

        private string ValidateAndGetJobId(string validatorId, string validatorInstanceId)
        {
            Guard.NotNullOrEmpty(validatorId, "validatorId", log);
            Guard.NotNullOrEmpty(validatorInstanceId, "validatorInstanceId", log);

            return JobIdUtils.GetJobId(validatorId, validatorInstanceId);
        }

        private void ValidateScheduledRequestsInputNotNull(IEnumerable<IScheduleRequest> scheduleRequests)
        {
            foreach (var scheduleRequest in scheduleRequests)
            {
                this.ValidateScheduledRequestInputNotNull(scheduleRequest);
            }
        }

        private void ValidateScheduledRequestInputNotNull(IScheduleRequest scheduleRequest)
        {
            Guard.NotNullOrEmpty(scheduleRequest.ValidatorId, "scheduleRequest.ValidatorId", log);
            Guard.NotNullOrEmpty(scheduleRequest.ValidatorInstanceId, "scheduleRequest.ValidatorInstanceId", log);
            Guard.NotNullOrEmpty(scheduleRequest.CronTabExpression, "scheduleRequest.CronTabExpression", log);
        }

        public void Dispose()
        {
            this._hangfireValidationServer.Dispose();
        }
    }
}