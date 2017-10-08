using MassTransit;
using PrecisionDiscovery.Diagnostics;
using PrecisionDiscovery.Messaging;
using PrecisionDiscovery.Messaging.Messages;
using System;
using System.Security.Principal;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace ValidationServer.Notification
{
    public class NotificationValidationResultHandler : IValidationResultHandler
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        private IPublishEndpoint _publishEndpoint;

        public NotificationValidationResultHandler(IPublishEndpoint publishEndpoint)
        {
            this._publishEndpoint = Guard.NotNull(publishEndpoint, "publishEndpoint", log);
        }

        public string Name
        {
            get
            {
                return "NotificationValidationResultHandler";
            }
        }

        public void OutputValidatorResult(IValidatorRunEntry validatorRunEntry)
        {
            NotificationSeverity severity = NotificationSeverity.Unknown;
            string description = string.Empty;

            string module = string.Join(".", validatorRunEntry.FilterSequence);
            string action = string.Join(".", validatorRunEntry.Result.FilterSequence);

            switch (validatorRunEntry.Result.ResultCode)
            {
                case ValidatorResultCode.Warning:
                    {
                        severity = NotificationSeverity.Warn;
                        description = ValidatorDescriptionBuilder.BuildWarningDescription(validatorRunEntry);
                        break;
                    }
                case ValidatorResultCode.Error:
                    {
                        severity = NotificationSeverity.Error;
                        description = ValidatorDescriptionBuilder.BuildErrorDescription(validatorRunEntry);
                        break;
                    }
                case ValidatorResultCode.Fatal:
                    {
                        severity = NotificationSeverity.Critical;
                        description = ValidatorDescriptionBuilder.BuildFatalDescription(validatorRunEntry);
                        break;
                    }                
                default:
                    {
                        severity = NotificationSeverity.Info;
                        description = ValidatorDescriptionBuilder.BuildSuccessDescription(validatorRunEntry);
                        break;
                    }
            }

            var notification = new NotificationMessage()
            {
                ApplicationId = "ValidationServer",
                NotificationTitle = string.Format("{0} Validation Results", validatorRunEntry.Result.ValidatorId),
                PhysicalSource = Environment.MachineName,
                UserId = WindowsIdentity.GetCurrent().Name,
                Severity = severity,
                Action = action,
                Module = module,
                Description = description
            } as INotificationMessage;

            _publishEndpoint.Publish(notification);
        }
    }
}