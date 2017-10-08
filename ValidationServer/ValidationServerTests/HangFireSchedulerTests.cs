using Hangfire;
using Hangfire.MemoryStorage;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationSchedulerData.Scheduler;
using ValidationServer.Data.Scheduler;
using ValidationServer.Scheduler;
using Xunit;

namespace ValidationServerTests
{
    /// <summary>
    /// Test HangFire scheduler. This class uses in memory storage for tests: https://github.com/perrich/Hangfire.MemoryStorage
    /// </summary>
    public class HangFireSchedulerTests : IDisposable
    {
        private const string CronTabExpressionRunHourly = "0 * * * *";
        private const int MaxValidationsToSchedule = 10;

        private BackgroundJobServer _backGroundJobServer;

        private HangFireScheduler _hangFireScheduler;

        public HangFireSchedulerTests()
        {
            //Initialize in memory storage background job server
            GlobalConfiguration.Configuration.UseMemoryStorage();

            //Can't inject BackgroundJobServer into tests b/c memory storage has to be initialized before instantiation
            this._backGroundJobServer = new BackgroundJobServer();

            this.InitializeHangFireScheduler();
        }

        public void Dispose()
        {
            //The in memory storage lags a little on the removes. Remove items from list so tests don't impact each other.
            this.RemoveScheduledValidations();

            this._backGroundJobServer.Dispose();
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireSchedulerThrowsOnNullConstructorParams(IScheduleCallback scheduledCallBack)
        {
            Assert.ThrowsAny<Exception>(() => { HangFireScheduler hfs = new HangFireScheduler(null, scheduledCallBack); });
            Assert.ThrowsAny<Exception>(() => { HangFireScheduler hfs = new HangFireScheduler(this._backGroundJobServer, null); });
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireSchedulerAddValidationToScheduleThrowsOnNullParam()
        {
            Assert.Throws<ArgumentNullException>(() => { this._hangFireScheduler.AddValidationToSchedule(null); });           

            Assert.ThrowsAny<ArgumentNullException>(() => { this._hangFireScheduler.AddValidationsToSchedule(null); });
        }

        [Theory, AutoData_NSubstitute]
        public void ScheduleRequestsThrowsOnNullParam(string validatorId, string validatorInstanceId, string cronTabExpression)
        {
            Assert.ThrowsAny<Exception>(() => { new ScheduleRequest("", validatorInstanceId, cronTabExpression); });
            Assert.ThrowsAny<Exception>(() => { new ScheduleRequest(validatorId, "", cronTabExpression); });
            Assert.ThrowsAny<Exception>(() => { new ScheduleRequest(validatorId, validatorInstanceId, ""); });
        }

        [Theory, AutoData_NSubstitute]
        public void ScheduleRequestsSucceedsWithParam(string validatorId, string validatorInstanceId, string cronTabExpression)
        {
            Assert.NotNull(new ScheduleRequest(validatorId, validatorInstanceId, cronTabExpression));
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireItemScheduledSuccessfully(string validatorId, string validatorInstanceId)
        {
            IScheduleRequest scheduleRequest = new ScheduleRequest(validatorId, validatorInstanceId, CronTabExpressionRunHourly);
            IScheduledValidation scheduledValidation = this._hangFireScheduler.AddValidationToSchedule(scheduleRequest);
            var scheduledValidations = this._hangFireScheduler.GetScheduledValidations();
            var isScheduled = this._hangFireScheduler.IsValidationScheduled(validatorId, validatorInstanceId);

            Assert.Equal(scheduledValidation.JobId, JobIdUtils.GetJobId(validatorId, validatorInstanceId));
            Assert.Equal(1, scheduledValidations.Count());
            Assert.True(isScheduled);
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireMultipleItemScheduledSuccessfully()
        {
            IEnumerable<IScheduleRequest> scheduledRequests = this.GetListOfScheduleRequests(MaxValidationsToSchedule);
            IEnumerable<IScheduledValidation> scheduledValidation = this._hangFireScheduler.AddValidationsToSchedule(scheduledRequests);
            var scheduledValidations = this._hangFireScheduler.GetScheduledValidations();

            Assert.Equal(scheduledRequests.Count(), scheduledValidations.Count());
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireSchedulerEditValidationToScheduleThrowsOnNullParam()
        {
            Assert.ThrowsAny<Exception>(() => { this._hangFireScheduler.EditValidationSchedule(null); });
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireItemEditNonExistingScheduleThrows(string validatorId, string validatorInstanceId)
        {
            IScheduleRequest scheduleRequest = new ScheduleRequest(validatorId, validatorInstanceId, CronTabExpressionRunHourly);
            Assert.ThrowsAny<Exception>(() => this._hangFireScheduler.EditValidationSchedule(scheduleRequest));
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireItemRemoveScheduledItemSucceeds([Frozen]string validatorId, [Frozen]string validatorInstanceId)
        {
            IScheduleRequest scheduleRequest = new ScheduleRequest(validatorId, validatorInstanceId, CronTabExpressionRunHourly);
            IScheduledValidation val = this._hangFireScheduler.AddValidationToSchedule(scheduleRequest);

            var scheduledValidations = this._hangFireScheduler.GetScheduledValidations();

            Assert.Equal(1, scheduledValidations.Count());

            this._hangFireScheduler.RemoveValidationFromSchedule(validatorId, validatorInstanceId);
            scheduledValidations = this._hangFireScheduler.GetScheduledValidations();
            var isScheduled = this._hangFireScheduler.IsValidationScheduled(validatorId, validatorInstanceId);

            Assert.Equal(0, scheduledValidations.Count());
            Assert.False(isScheduled);
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireItemScheduledCronTabInvalid(string validatorId, string validatorInstanceId, string cronTabExpressionInvalid)
        {
            IScheduleRequest scheduleRequest = new ScheduleRequest(validatorId, validatorInstanceId, cronTabExpressionInvalid);
            Assert.ThrowsAny<Exception>(() => this._hangFireScheduler.AddValidationToSchedule(scheduleRequest));
        }

        [Theory, AutoData_NSubstitute]
        public void HangFireGetScheduledItemRange()
        {
            int itemsToSchedule = 5;
            int itemsToGet = 2;

            IEnumerable<IScheduleRequest> scheduledRequests = this.GetListOfScheduleRequests(itemsToSchedule);
            this._hangFireScheduler.AddValidationsToSchedule(scheduledRequests);

            IEnumerable<IScheduledValidation> scheduledValidations =
                            this._hangFireScheduler.GetScheduledValidations(itemsToSchedule - itemsToGet, scheduledRequests.Count());

            Assert.Equal(itemsToGet, scheduledValidations.Count());

            var itemIndex = itemsToSchedule - itemsToGet;
            this.ValidateScheduledRequest(scheduledRequests.ElementAt(itemIndex), scheduledValidations.ElementAt(0));
            this.ValidateScheduledRequest(scheduledRequests.ElementAt(itemIndex + 1), scheduledValidations.ElementAt(1));
        }


        private IEnumerable<IScheduleRequest> GetListOfScheduleRequests(int count)
        {
            List<IScheduleRequest> requests = new List<IScheduleRequest>();

            for (int i = 0; i < count; i++)
            {
                var validatorId = string.Format("ValidationId{0}", i);
                var validatorInstanceId = string.Format("ValidationInstanceId{0}", i);
                var request = new ScheduleRequest(validatorId, validatorInstanceId, CronTabExpressionRunHourly) as IScheduleRequest;
                requests.Add(request);
            }

            return requests;
        }

        private void RemoveScheduledValidations()
        {
            IEnumerable<IScheduledValidation> schduledValidations = this._hangFireScheduler.GetScheduledValidations();

            foreach (var schduledValidation in schduledValidations)
            {
                var instanceInfo = JobIdUtils.GetInstanceInfo(schduledValidation.JobId);
                this._hangFireScheduler.RemoveValidationFromSchedule(instanceInfo.ValidatorId, instanceInfo.ValidatorInstanceId);
            }
        }

        private void ValidateScheduledRequest(IScheduleRequest scheduleRequest, IScheduledValidation scheduledValidation)
        {
            var jobId = scheduledValidation.JobId;
            var instanceInfo = JobIdUtils.GetInstanceInfo(jobId);
            Assert.Equal(scheduleRequest.ValidatorId, instanceInfo.ValidatorId);
            Assert.Equal(scheduleRequest.ValidatorInstanceId, instanceInfo.ValidatorInstanceId);
        }

        private void InitializeHangFireScheduler()
        {
            IScheduleCallback scheduledCallBack = new HangFireScheduleCallback();
            scheduledCallBack.RunValidatorEvent += ScheduledCallBack_RunValidatorEvent1;
            this._hangFireScheduler = new HangFireScheduler(this._backGroundJobServer, scheduledCallBack);
        }

        private void ScheduledCallBack_RunValidatorEvent1(object sender, ValidatorInfoArgs e)
        {
            //Do nothing
        }
    }
}