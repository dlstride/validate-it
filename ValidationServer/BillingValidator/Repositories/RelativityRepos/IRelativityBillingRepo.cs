using BillingValidator.Models.Relativity;
using System;
using System.Collections.Generic;

namespace BillingValidator.Repositories.RelativityRepos
{
    public interface IRelativityBillingRepo
    {
        IEnumerable<CaseStatistics> GetCaseStatistics(DateTime date);
        IEnumerable<CaseStatisticsMonthly> GetCaseStatisticsMonthly(DateTime date);
    }
}
