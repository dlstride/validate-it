using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationServer.Data.Validators;

namespace ValidationServerTests.TestHelpers
{
   public class TestValidatorInstance : IValidatorInstance
    {

        public static string TestValidatorInstanceId = "TestValidatorInstanceId";
        public string Description
        {
            get
            {
                return "TestValidatorInstance";
            }
        }

        public string Name
        {
            get
            {
                return "TestValidatorInstance";
            }
        }

        public string ValidatorInstanceId
        {
            get
            {
                return TestValidatorInstanceId;
            }
        }
    }
}
