using ValidationServer.Scheduler;

namespace ValidationServerTests.TestHelpers
{
    public class TestScheduleCallBack : IScheduleCallback
    {
        public event CallBackToValidationServer RunValidatorEvent;

        public void ScheduleValidator(string jobId, string cronTabExpression)
        {
        }
    }
}