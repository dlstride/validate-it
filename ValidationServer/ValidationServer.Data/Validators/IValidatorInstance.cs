namespace ValidationServer.Data.Validators
{
    public interface IValidatorInstance
    {
        string Name { get; }

        string ValidatorInstanceId { get; }

        string Description { get; }
    }
}
