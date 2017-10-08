using MassTransit;
using Ninject;
using Ninject.Extensions.ChildKernel;
using NSubstitute;
using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using ValidationSchedulerData.Scheduler;
using ValidationServer.Bootstrap;
using ValidationServer.Data.Validators;
using ValidationServer.Interfaces;
using ValidationServer.Notification;
using ValidationServer.Scheduler;
using ValidationServer.Scheduler.Scheduler;
using ValidationServerTests.TestHelpers;
using Xunit;

namespace ValidationServer.Tests
{
    public class ValidationServerTests
    {
        [Theory, AutoData_NSubstitute]
        public void ValidationServerInitWithNullParametersThrow(IValidatorProvider validatorProvider, IValidatorRunner validatorRunner, IValidationScheduler validationScheduler, IScheduleCallback scheduleCallbacker, IValidationResultHandler[] notificationHandler, IPublishEndpoint publishEndpoint)
        {
            Assert.ThrowsAny<Exception>(
                () =>
                {
                    ValidationServer vs = new ValidationServer(null, validatorRunner, validationScheduler, scheduleCallbacker, publishEndpoint, notificationHandler);
                });

            Assert.ThrowsAny<Exception>(
                () =>
                {
                    ValidationServer vs = new ValidationServer(validatorProvider, null, validationScheduler, scheduleCallbacker, publishEndpoint, notificationHandler);
                });

            Assert.ThrowsAny<Exception>(
                () =>
                {
                    ValidationServer vs = new ValidationServer(validatorProvider, validatorRunner, null, scheduleCallbacker, publishEndpoint, notificationHandler);
                });

            Assert.ThrowsAny<Exception>(
                () =>
                {
                    ValidationServer vs = new ValidationServer(validatorProvider, validatorRunner, validationScheduler, null, publishEndpoint, notificationHandler);
                });

            Assert.ThrowsAny<Exception>(
                () =>
                {
                    ValidationServer vs = new ValidationServer(validatorProvider, validatorRunner, validationScheduler, scheduleCallbacker, null, notificationHandler);
                });

            Assert.ThrowsAny<Exception>(
                () =>
                {
                    ValidationServer vs = new ValidationServer(validatorProvider, validatorRunner, validationScheduler, scheduleCallbacker, publishEndpoint, null);
                });
        }

        [Theory, AutoData_NSubstitute]
        public void ValidationServerInitWithNonNullParametersSucceeds(TestValidatorProvider validatorProvider, IValidatorRunner validatorRunner, IValidationScheduler validationScheduler, IScheduleCallback scheduleCallbacker, IValidationResultHandler[] notificationHandler, IPublishEndpoint publishEndpoint)
        {
            Assert.NotNull(new ValidationServer(validatorProvider, validatorRunner, validationScheduler, scheduleCallbacker, publishEndpoint, notificationHandler));
        }


        [Theory, AutoData_NSubstitute]
        public void AddScheduleToValidationServerVerificationWorks(IValidatorRunner validatorRunner, IScheduleCallback scheduleCallbacker, IValidationScheduler validatorScheduler, IValidatorProvider validatorProvider, IList<IValidatorInstance> validatorInstancesToReturn, string unknownInstanceId, string includedAndNotScheduledValidatorIdInstanceId,
            string includedAndScheduledValidatorIdInstanceId, string includedAndScheduledValidatorId, string includedAndNotScheduledValidatorId, string unknownProxyId, IList<IValidatorProxy> validatorProxiesToReturn, IValidationResultHandler[] notificationHandler, IPublishEndpoint publishEndpoint)
        {
            var includedProxy = Substitute.For<IValidatorProxy>();
            var includedInstance = Substitute.For<IValidatorInstance>();
            includedProxy.ValidatorId.ReturnsForAnyArgs(includedAndNotScheduledValidatorId);
            includedInstance.ValidatorInstanceId.ReturnsForAnyArgs(includedAndNotScheduledValidatorIdInstanceId);

            var includedAndScheduledProxy = Substitute.For<IValidatorProxy>();
            var includedAndScheduledInstance = Substitute.For<IValidatorInstance>();
            includedAndScheduledProxy.ValidatorId.ReturnsForAnyArgs(includedAndScheduledValidatorId);
            includedAndScheduledInstance.ValidatorInstanceId.ReturnsForAnyArgs(includedAndScheduledValidatorIdInstanceId);

            validatorProvider.GetValidatorProxy(includedProxy.ValidatorId).Returns(includedProxy);
            validatorProvider.GetValidatorProxy(includedAndScheduledProxy.ValidatorId).Returns(includedAndScheduledProxy);

            validatorProvider.GetValidatorInstance(includedAndScheduledValidatorId, includedAndScheduledValidatorIdInstanceId).Returns(includedAndScheduledInstance);

            validatorProvider.GetValidatorInstance(includedAndNotScheduledValidatorId,includedAndNotScheduledValidatorIdInstanceId).Returns(includedInstance);

            validatorScheduler.IsValidationScheduled(includedAndNotScheduledValidatorId, includedAndNotScheduledValidatorIdInstanceId).Returns(false);

            validatorScheduler.IsValidationScheduled(includedAndScheduledValidatorId, includedAndScheduledValidatorIdInstanceId).Returns(true);


            validatorProvider.When(x => x.GetValidatorProxy(Arg.Is<string>(d => d != includedAndNotScheduledValidatorId && d != includedAndScheduledValidatorId))).Do(g => { throw new Exception(); });

            validatorProvider.When(x => x.GetValidatorInstance(Arg.Any<string>(), Arg.Is<string>(d => d != includedAndNotScheduledValidatorIdInstanceId && d != includedAndScheduledValidatorIdInstanceId))).Do(g => { throw new Exception(); });

            var vs = new ValidationServer(validatorProvider, validatorRunner, validatorScheduler, scheduleCallbacker, publishEndpoint, notificationHandler);

            ScheduleRequest validRequest = new ScheduleRequest(includedProxy.ValidatorId, includedInstance.ValidatorInstanceId, "* * * * *");
            ScheduleRequest requestWithInValidInstance = new ScheduleRequest(includedProxy.ValidatorId, unknownInstanceId, "* * * * *");
            ScheduleRequest requestWithInValidProxy = new ScheduleRequest(unknownProxyId, unknownInstanceId, "* * * * *");
            ScheduleRequest alreadyScheduledRequest = new ScheduleRequest(includedAndScheduledValidatorId, includedAndScheduledValidatorIdInstanceId, "* * * * *");

            //This do not throw - Pass Verification
            vs.AddValidationsToSchedule(new List<ScheduleRequest>() { validRequest });

            //This throw cause proxy is not available
            Assert.ThrowsAny<Exception>(() => vs.AddValidationsToSchedule(new List<ScheduleRequest>() { requestWithInValidInstance }));

            //This throws cause instance is not available
            Assert.ThrowsAny<Exception>(()=>vs.AddValidationsToSchedule(new List<ScheduleRequest>() { requestWithInValidInstance }));

            //This throws cause item is already scheduled is not available
            Assert.ThrowsAny<Exception>(() => vs.AddValidationsToSchedule(new List<ScheduleRequest>() { alreadyScheduledRequest }));
        }


        [Theory, AutoData_NSubstitute]
        public void TestQueueValidatorFailsWhenNullParameter(IValidatorRunner validatorRunner, IScheduleCallback scheduleCallbacker, IValidationScheduler validatorScheduler, IValidatorProvider validatorProvider, IList<IValidatorInstance> validatorInstancesToReturn,
            IValidationResultHandler[] notificationHandler, IPublishEndpoint publishEndpoint)
        {
            var vs = new ValidationServer(validatorProvider, validatorRunner, validatorScheduler, scheduleCallbacker, publishEndpoint, notificationHandler);
            Assert.ThrowsAny<Exception>(() => vs.QueueValidatorForExecution(null));
        }

        [Theory, AutoData_NSubstitute]
        public void TestQueueValidatorRunsCorrectly(ValidatorInstanceInfo info, IValidatorRunner validatorRunner, IScheduleCallback scheduleCallbacker, IValidationScheduler validatorScheduler, IValidatorProvider validatorProvider, IList<IValidatorInstance> validatorInstancesToReturn,
            IValidationResultHandler[] notificationHandler, IPublishEndpoint publishEndpoint)
        {
            var vs = new ValidationServer(validatorProvider, validatorRunner, validatorScheduler, scheduleCallbacker, publishEndpoint, notificationHandler);
            vs.QueueValidatorForExecution(info);
        }

        private IChildKernel GetChildKernelWithBindings(IKernel kernel)
        {
            IChildKernel childKernel = new ChildKernel(kernel);

            childKernel.Bind<IVasaClient>().ToMethod(config =>
            {
                return new VasaClient();
            });

            return childKernel;
        }

        private IKernel GetTestKernel()
        {
            IKernel parentKernel = new StandardKernel();

            parentKernel.Bind<IVasaClient>().ToMethod(config =>
            {
                return Substitute.For<IVasaClient>();
            }).InSingletonScope();

            parentKernel.Bind<IChildKernelCreator>().To<TestChildKernelCreator>().InSingletonScope();

            parentKernel.Bind<IValidatorProvider>().To<IValidatorProvider>().InSingletonScope();

            parentKernel.Bind<IValidationServer>().To<ValidationServer>().InSingletonScope();

            parentKernel.Bind<IValidatorRunner>().To<TestValidatorRunner>().InSingletonScope();

            parentKernel.Bind<IScheduleCallback>().To<TestScheduleCallBack>().InSingletonScope();

            return parentKernel;
        }
    }
}