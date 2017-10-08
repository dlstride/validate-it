using System;
using System.Collections.Generic;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace TemplateValidator.Validators
{
    public class TemplateTestValidator : Validator, IValidator
    {
        public TemplateTestValidator() :
            base(TemplateValidatorProxy.TemplateValidatorId)
        {
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Info("TemplateTestValidator Execute started");

            List<IValidatorResult> validatorResults = new List<IValidatorResult>();

            var vr = new ValidatorResult(this.ValidatorProxyId, "TemplateTestValidator - template expected for validator ", ValidatorResultCode.Success, "TemplateTestValidator");
            validatorResults.Add(vr);

            log.Info("Starting TemplateTestValidator Execute completed with {ResultCount} results", validatorResults.Count);

            string Value1 = "$MyParameter1";
            Console.WriteLine(Value1);
            return validatorResults;
        }
    }
}