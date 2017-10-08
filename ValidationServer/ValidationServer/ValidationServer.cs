using MassTransit;
using PrecisionDiscovery.Diagnostics;
using PrecisionDiscovery.Messaging;
using PrecisionDiscovery.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ValidationServer.Data.Scheduler;
using ValidationServer.Data.Validators;
using ValidationServer.Interfaces;
using ValidationServer.Notification;
using ValidationServer.Scheduler;
using ValidationServer.Scheduler.Scheduler;

namespace ValidationServer
{
    public class ValidationServer : IValidationServer
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public const string ValidatorModulesParamName = "validatorModules";

        private IPublishEndpoint _publishEndpoint;
        private IValidationResultHandler[] _notificationHandlers;
        private IValidatorProvider _validatorProvider;
        private IValidatorRunner _validatorRunner;
        private IValidationScheduler _validationScheduler;
        private IScheduleCallback _scheduleCallback;

        private const string UnhandledExceptionResultCode = "Unhandled Exception";

        public ValidationServer(IValidatorProvider validatorProvider, IValidatorRunner validatorRunner, IValidationScheduler validationScheduler, IScheduleCallback scheduleCallback, IPublishEndpoint publishEndpoint, IValidationResultHandler[] notificationHandlers)
        {
            this._publishEndpoint = Guard.NotNull(publishEndpoint, "publishEndpoint", log);
            this._validatorProvider = Guard.NotNull(validatorProvider, "validatorProvider", log);
            this._validatorRunner = Guard.NotNull(validatorRunner, "validatorRunner", log);
            this._validationScheduler = Guard.NotNull(validationScheduler, "validationScheduler", log);
            this._scheduleCallback = Guard.NotNull(scheduleCallback, "scheduleCallback", log);

            this._notificationHandlers = Guard.NotNull(notificationHandlers, "notificationHandlers", log);
            this._scheduleCallback.RunValidatorEvent += _scheduleCallback_RunValidatorEvent;
            this._validatorRunner.ValidatorResultsReady += _validatorRunner_ValidatorResultsReady;
        }

        private void _validatorRunner_ValidatorResultsReady(object sender, ValidatorResultsArg e)
        {
            SendValidatorResults(e.ValidatorResults);
        }

        private void _scheduleCallback_RunValidatorEvent(object sender, ValidatorInfoArgs e)
        {
            this.QueueValidatorForExecution(e.ValidatorInfo);
        }

        #region External API 

        public IEnumerable<IScheduledValidation> AddValidationsToSchedule(IEnumerable<IScheduleRequest> scheduleRequests)
        {
            Guard.NotNull(scheduleRequests, "scheduleRequests", log);

            log.Debug("Validation Server AddValidationsToSchedule: {ValidationCount} validations being scheduled", scheduleRequests.Count());

            IEnumerable<IScheduledValidation> results = null;

            try
            {
                this.VerifyScheduleRequestValidity(scheduleRequests);
                this.VerifyNoDuplicateValidatorSchedules(scheduleRequests);
                results = this._validationScheduler.AddValidationsToSchedule(scheduleRequests);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return results;
        }

        private void VerifyScheduleRequestValidity(IEnumerable<IScheduleRequest> scheduleRequests)
        {
            foreach (var scheduleRequest in scheduleRequests)
            {
                IValidatorProxy proxy = this._validatorProvider.GetValidatorProxy(scheduleRequest.ValidatorId);
                IValidatorInstance instance = this._validatorProvider.GetValidatorInstance(scheduleRequest.ValidatorId, scheduleRequest.ValidatorInstanceId);

                bool jobExists = this._validationScheduler.IsValidationScheduled(proxy.ValidatorId, instance.ValidatorInstanceId);

                if (jobExists)
                {
                    throw new Exception(string.Format("Validation already scheduled for validator instance {0}", scheduleRequest.ValidatorInstanceId));
                }
            }
        }

        private void VerifyNoDuplicateValidatorSchedules(IEnumerable<IScheduleRequest> scheduleRequests)
        {
            var duplicateValidatorInstanceSchedules = scheduleRequests.GroupBy(s => new { s.ValidatorId, s.ValidatorInstanceId }).SelectMany(grp => grp.Skip(1));

            if (duplicateValidatorInstanceSchedules.Count() > 0)
            {
                foreach (var duplicate in duplicateValidatorInstanceSchedules)
                {
                    log.Debug("ValidationServer VerifyNoDuplicateValidatorSchedules - multiple validator schedules found for ValidatorId {ValidatorId} and ValidatorInstanceId {ValidatorInstanceId}", 
                                    duplicate.ValidatorId, duplicate.ValidatorInstanceId);
                }

                var duplicateInstanceId = duplicateValidatorInstanceSchedules.Select(s => s.ValidatorInstanceId);
                throw new Exception(string.Format("Multiple schedules found for validator instance(s): {0}", string.Join(", ", duplicateInstanceId)));
            }
        }

        #endregion

        public void QueueValidatorForExecution(ValidatorInstanceInfo info)
        {
            Guard.NotNull(info, "info", log);

            _validatorRunner.QueueValidatorForExecution(info);
        }

        private void SendValidatorResults(IList<IValidatorRunEntry> validatorRunEntries)
        {
            foreach (var notificationHandler in this._notificationHandlers)
            {
                log.Debug("NotificationHandler: {name} will now send notifications for {count} ValidationRunEntries", notificationHandler.Name, validatorRunEntries.Count);

                foreach (var validatorRunEntry in validatorRunEntries)
                {
                    notificationHandler.OutputValidatorResult(validatorRunEntry);
                }
            }
        }
    }
}