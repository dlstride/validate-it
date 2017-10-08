using BillingValidator.Models.Relativity;
using BillingValidator.Repositories.RelativityRepos;
using BillingValidator.Validators;
using Ninject;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using PrecisionDiscovery.Configuration.Vasa;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;
using Xunit;

namespace BillingValidatorTests
{
    public class BillingValidatorTests
    {
        [Theory, AutoData_NSubstitute]
        public void ValidatorRelativityProcessFailsWhenNoCaseStatsBindingFound(BillingValidatorSettings billingSettings, [Frozen]IRelativityBillingRepo relBillingRepo, [Frozen] IRelativityProvidersRepo relProvidersRepo, IValidatorContext validatorContext)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();
            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);
            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Error).Count(), 4);
        }

        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProcessorFailsWhenCaseStatsNotFoundRowsFound(IValidatorContext validatorContext, IList<DataCollectionsRuns> dataCollectionRunsList, IList<UserCollectionsRuns> userCollectionRunsList, BillingValidatorSettings billingSettings)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatistics>());
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatisticsMonthly>());

            relProvidersRepo.GetDataCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(dataCollectionRunsList);
            relProvidersRepo.GetUserCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(userCollectionRunsList);

            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);
            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);
            Assert.Equal( results.Where( r=> r.ResultCode == ValidatorResultCode.Error).Count(), 2);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Success).Count(), 2);          
        }
        

        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProcessorFailsWhenCaseStatisticsRowNotFound(IList<CaseStatistics> caseStatsList,
            IValidatorContext validatorContext, IList<DataCollectionsRuns> dataCollectionRunsList, IList<UserCollectionsRuns> userCollectionRunsList, BillingValidatorSettings billingSettings)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsList);
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatisticsMonthly>());

            relProvidersRepo.GetDataCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(dataCollectionRunsList);
            relProvidersRepo.GetUserCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(userCollectionRunsList);

            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);
            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Error).Count(), 1);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Success).Count(), 3);
        }

        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProcessorFailsWhenNoCaseStatisticsMonthlyRowFound(IValidatorContext validatorContext, IList<CaseStatistics> caseStatsList, 
            IList<DataCollectionsRuns> dataCollectionRunsList, IList<UserCollectionsRuns> userCollectionRunsList, BillingValidatorSettings billingSettings)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsList);
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatisticsMonthly>());

            relProvidersRepo.GetDataCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(dataCollectionRunsList);
            relProvidersRepo.GetUserCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(userCollectionRunsList);

            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);
            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Error).Count(), 1);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Success).Count(), 3);
        }
       
        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProcessorCaseStatisticsRowsFoundSucceeds(IList<CaseStatisticsMonthly> caseStatsMonthlyList, IList<CaseStatistics> caseStatsList,
            List<ConfigSection> fakeSections, IList<DataCollectionsRuns> dataCollectionRunsList, IList<UserCollectionsRuns> userCollectionRunsList, BillingValidatorSettings billingSettings)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();

            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsList);
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsMonthlyList);

            relProvidersRepo.GetDataCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(dataCollectionRunsList);
            relProvidersRepo.GetUserCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(userCollectionRunsList);

            var validatorContext = new ValidatorContext();
            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);

            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Success).Count(), 4);
        }

        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProcessorFailsWhenCaseStatsThrowsException(IValidatorContext validatorContext, BillingValidatorSettings billingSettings)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(c => { throw new Exception("An exception occurred."); });
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatisticsMonthly>());

            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);

            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);

            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Error).Count(), 4);
        }

        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProviderDataCollectionRunsTestFailsCausesValidatorError(BillingValidatorSettings billingSettings, IList<CaseStatisticsMonthly> caseStatsMonthlyList, IList<CaseStatistics> caseStatsList, IValidatorContext validatorContext,
            IList<UserCollectionsRuns> userCollectionRunsList)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsList);
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsMonthlyList);

            relProvidersRepo.GetDataCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<DataCollectionsRuns>());
            relProvidersRepo.GetUserCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(userCollectionRunsList);

            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);

            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);

            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Error).Count(), 1);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Success).Count(), 3);
        }

        [Theory, AutoData_NSubstitute]
        public void ValidateRelativityProviderUserCollectionRunsTestFailsCausesValidatorError(IList<CaseStatisticsMonthly> caseStatsMonthlyList, IList<CaseStatistics> caseStatsList, IValidatorContext validatorContext, IList<DataCollectionsRuns> dataCollectionRunsList, BillingValidatorSettings billingSettings)
        {
            var kernel = this.GetBillingValidatorWithAllKernelBindings();

            var relBillingRepo = kernel.Get<IRelativityBillingRepo>();
            var relProvidersRepo = kernel.Get<IRelativityProvidersRepo>();
            var billingValidator = new BillingRanValidator(relBillingRepo, relProvidersRepo, billingSettings);

            relBillingRepo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsList);
            relBillingRepo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(caseStatsMonthlyList);

            relProvidersRepo.GetDataCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(dataCollectionRunsList);
            relProvidersRepo.GetUserCollectionsRuns(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<UserCollectionsRuns>());

            IList<IValidatorResult> results = billingValidator.Execute(validatorContext);

            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Error).Count(), 1);
            Assert.Equal(results.Where(r => r.ResultCode == ValidatorResultCode.Success).Count(), 3);
        }

        private IKernel GetBillingValidatorWithAllKernelBindings(bool setupValidatorTestsBindings = true)
        {
            IKernel kernel = new StandardKernel();

            kernel.Bind<IVasaClient>().ToMethod(config =>
            {
                return new VasaClient();
            });

            kernel.Bind<IRelativityBillingRepo>().ToMethod(c => { return Substitute.For<IRelativityBillingRepo>(); }).InSingletonScope();
            kernel.Bind<IRelativityProvidersRepo>().ToMethod(c => { return Substitute.For<IRelativityProvidersRepo>(); }).InSingletonScope();

            kernel.Bind<BillingRanValidator>().ToSelf();

            return kernel;
        }       
    }
}