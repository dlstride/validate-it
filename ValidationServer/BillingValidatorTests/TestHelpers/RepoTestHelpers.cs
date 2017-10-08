using BillingValidator.Models.Relativity;
using BillingValidator.Repositories.RelativityRepos;
using BillingValidator.Repositories.RelativityReposs;
using NSubstitute;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;

namespace BillingValidatorTests.TestHelpers
{
    public static class RepoTestHelpers
    {
        public static IRelativityBillingRepo CreateRelativityBillingRepo()
        {
            Fixture fix = new Fixture();
            var repo = fix.Create<RelativityBillingRepo>();
            repo.GetCaseStatistics(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatistics>());
            repo.GetCaseStatisticsMonthly(Arg.Any<DateTime>()).ReturnsForAnyArgs(new List<CaseStatisticsMonthly>());
            return repo;
        }

        public static IList<DataCollectionsRuns> ReturnDataCollectionRunsList()
        {
            Fixture fix = new Fixture();
            return fix.Create<IList<DataCollectionsRuns>>();
        }
    }
}