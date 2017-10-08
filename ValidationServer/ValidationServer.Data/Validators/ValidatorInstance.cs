namespace ValidationServer.Data.Validators
{
    public class ValidatorInstance : IValidatorInstance
    {
        public ValidatorInstance(string validatorId, string name, string description)
        {
            this.Name = name; //TODO move Guard.NotNullOrEmpty(name, "name", log);
            this.Description = description;
            this.ValidatorInstanceId = validatorId;
        }

        public string Name { get; }

        public string Description { get; }

        public string ValidatorInstanceId
        {
            get;
        }
    }
}
