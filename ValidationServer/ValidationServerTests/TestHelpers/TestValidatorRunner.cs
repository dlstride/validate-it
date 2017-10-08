using ValidationServer;
using ValidationServer.Interfaces;
using ValidationServer.Scheduler.Scheduler;

namespace ValidationServerTests.TestHelpers
{
    public class TestValidatorRunner : IValidatorRunner
    {
        public event ReturnValidatorResults ValidatorResultsReady;

        public void QueueValidatorForExecution(ValidatorInstanceInfo info)
        {
            
        }
    }
}
