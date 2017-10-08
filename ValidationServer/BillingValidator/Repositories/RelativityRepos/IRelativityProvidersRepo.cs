using BillingValidator.Models.Relativity;
using System;
using System.Collections.Generic;

namespace BillingValidator.Repositories.RelativityRepos
{
    public interface IRelativityProvidersRepo
    {
        IEnumerable<UserCollectionsRuns> GetUserCollectionsRuns(DateTime date);
        IEnumerable<DataCollectionsRuns> GetDataCollectionsRuns(DateTime date);
    }
}
