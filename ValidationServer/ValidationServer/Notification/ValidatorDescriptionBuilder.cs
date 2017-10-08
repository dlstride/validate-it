using System.Text;
using ValidationServer.Data.Validators;

namespace ValidationServer.Notification
{
    public static class ValidatorDescriptionBuilder
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();      

        public static string BuildErrorDescription(IValidatorRunEntry validatorRunEntry)
        {
            string title = string.Format("Validator {0} ran with one or more errors", validatorRunEntry.Result.ValidatorId);
            return BuildDescription(title, validatorRunEntry);
        }

        public static string BuildFatalDescription(IValidatorRunEntry validatorRunEntry)
        {
            string title = string.Format("Validator {0} ran with one or more fatal errors", validatorRunEntry.Result.ValidatorId);
            return BuildDescription(title, validatorRunEntry);
        }

        public static string BuildSuccessDescription(IValidatorRunEntry validatorRunEntry)
        {
            string title = string.Format("Validator {0} ran successfully", validatorRunEntry.Result.ValidatorId);
            return BuildDescription(title, validatorRunEntry);
        }

        public static string BuildWarningDescription(IValidatorRunEntry validatorRunEntry)
        {
            string title = string.Format("Validator {0} ran with warnings", validatorRunEntry.Result.ValidatorId);
            return BuildDescription(title, validatorRunEntry);
        }

        public static string BuildDescription(string title, IValidatorRunEntry validatorRunEntry)
        {
            log.Debug("Building description for ValidatorRunEntry with tags {tags}", string.Join(".", validatorRunEntry.FilterSequence));
            var sb = new StringBuilder();

            sb.AppendLine(title);
            sb.AppendLine();
            sb.AppendLine(string.Format("Result Identifier: {0}", validatorRunEntry.Result.ResultIdentifier));
            sb.AppendLine(string.Format("Start Time: {0}", validatorRunEntry.StartTime));
            sb.AppendLine(string.Format("Finish Time: {0}", validatorRunEntry.FinishTime));
            sb.AppendLine(string.Format("Result Code: {0}", validatorRunEntry.Result.ResultCode));
            sb.AppendLine(string.Format("Description: {0}", validatorRunEntry.Result.Description));

            return sb.ToString();
        }
    }
}