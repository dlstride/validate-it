using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Data.Enumerations;

namespace ValidationServer.Data.Validators
{
    public class ValidatorResult : IValidatorResult
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ValidatorResult(string validatorId, string description, ValidatorResultCode resultCode, string resultIdentifier, List<string> tags = null)
        { 
            this.ValidatorId = Guard.NotNullOrEmpty(validatorId, "validatorId", log);

            this.Description = description;
            this.ResultCode = resultCode;
            this.ResultIdentifier = resultIdentifier;
            this.FilterSequence = tags ?? new List<string>();
        }

        public string ValidatorId { get; }

        public string Description { get; }

        public ValidatorResultCode ResultCode { get; }

        public string ResultIdentifier { get; }

        public IList<string> FilterSequence { get; }
    }
}
