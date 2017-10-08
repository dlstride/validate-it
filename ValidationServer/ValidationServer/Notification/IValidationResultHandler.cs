using ValidationServer.Data.Validators;

namespace ValidationServer.Notification
{
    public interface IValidationResultHandler
    {
        string Name { get; }
        void OutputValidatorResult(IValidatorRunEntry validatorRunEntry);
    }
}