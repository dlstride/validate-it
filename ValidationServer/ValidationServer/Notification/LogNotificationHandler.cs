using PrecisionDiscovery.IO;
using System;
using System.Text;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace ValidationServer.Notification
{
    public class LogValidationResultHandler : IValidationResultHandler
    {
        private const string ValidatationResultsFolderName = "ValidatorResults";

        private static Object locker = new Object();

        public string Name
        {
            get
            {
                return "LogValidationResultHandler";
            }
        }

        public void OutputValidatorResult(IValidatorRunEntry validatorRunEntry)
        {
            string results = this.GetValidatorResultNotiifyDescription(validatorRunEntry);
            if (!PDDirectory.Exists(ValidatationResultsFolderName))
            {
                PDDirectory.CreateDirectory(ValidatationResultsFolderName);
            }

            var fileName = string.Format(@"{0}\{1}_Output.txt", ValidatationResultsFolderName, validatorRunEntry.Result.ValidatorId);
            lock (locker)
            {
                System.IO.File.AppendAllText(fileName, results + Environment.NewLine);
            }
        }


        private string GetValidatorResultNotiifyDescription(IValidatorRunEntry validatorRunEntry)
        {
            var sb = new StringBuilder();
            switch (validatorRunEntry.Result.ResultCode)
            {
                case ValidatorResultCode.Warning:
                    {
                        sb.Append(ValidatorDescriptionBuilder.BuildWarningDescription(validatorRunEntry));
                        break;
                    }

                case ValidatorResultCode.Error:
                    {
                        sb.Append(ValidatorDescriptionBuilder.BuildErrorDescription(validatorRunEntry));
                        break;
                    }

                case ValidatorResultCode.Fatal:
                    {
                        sb.Append(ValidatorDescriptionBuilder.BuildFatalDescription(validatorRunEntry));
                        break;
                    }
                default:
                    {
                        sb.Append(ValidatorDescriptionBuilder.BuildSuccessDescription(validatorRunEntry));
                        break;
                    }
            }

            var action = string.Join(".", validatorRunEntry.Result.FilterSequence);

            sb.AppendLine();
            sb.AppendLine(string.Format("Module: {0}", string.Join(".", validatorRunEntry.FilterSequence)));
            sb.AppendLine(string.Format("Action: {0}", string.IsNullOrEmpty(action) ? "{NONE}" : action));
            sb.AppendLine();
            sb.AppendLine("-------------------------------------------------------");

            return sb.ToString();
        }
    }
}