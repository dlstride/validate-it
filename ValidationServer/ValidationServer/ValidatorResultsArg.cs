using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Data.Validators;

namespace ValidationServer
{
    public class ValidatorResultsArg : EventArgs
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ValidatorResultsArg(IList<IValidatorRunEntry> validatorResults)
        {
            Guard.NotNull(validatorResults, "validatorResults", log);
            this.ValidatorResults = validatorResults;
        }

        public IList<IValidatorRunEntry> ValidatorResults { get; }
    }
}
