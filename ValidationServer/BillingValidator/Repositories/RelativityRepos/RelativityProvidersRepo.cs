using BillingValidator.Models.Relativity;
using PetaPoco;
using PrecisionDiscovery.Configuration.Vasa;
using PrecisionDiscovery.Data;
using PrecisionDiscovery.Data.SQLServer;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingValidator.Repositories.RelativityRepos
{
    public class RelativityProvidersRepo : IRelativityProvidersRepo
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();
        private IVasaClient _gpodVasa;
        private const string UserCollectionRunsTableName = "[RelativityProvidersDB].[dbo].[UserCollectionRuns]";
        private const string DataCollectionRunsTableName = "[RelativityProvidersDB].[dbo].[DataCollectionRuns]";

        public RelativityProvidersRepo(IVasaClient gpodVasa)
        {
            this._gpodVasa = Guard.NotNull(gpodVasa, "gpodVasa", log);
        }

        private Database GetDatabase(IDBConnectionInfo connectionInfo)
        {
            var connectionString = ConnectionStringBuilder.GetConnnectionString(connectionInfo);
            Database db = new Database(connectionString, "System.Data.SqlClient");

            return db;
        }

        private IDBConnectionInfo GetRelativityProvidersDatabaseConnectionInfo()
        {
            string dataProvidersSection = "DataProviders.Relativity";

            log.Debug("BillingValidator: Building RelativityProvidersDatabaseConnectionInfo using Vasa: GPOD  Section: {section} ", dataProvidersSection);

            var section = _gpodVasa.GetConfigSections(null, dataProvidersSection);
            var connInfo = section.First().GetAsDBConnectionInfo();
            return connInfo;
        }

        public Database GetRelativityProvidersDatabase()
        {
            var connectionInfo = GetRelativityProvidersDatabaseConnectionInfo();
            return GetDatabase(connectionInfo);
        }

        public IEnumerable<UserCollectionsRuns> GetUserCollectionsRuns(DateTime date)
        {
            var db = GetRelativityProvidersDatabase();
            var query = string.Format("SELECT [ID],[ModifiedTime] FROM {0} where modifiedTime > '{1}'", UserCollectionRunsTableName, date);

            log.Debug("BillingValidator: Querying UserCollectionsRuns for Date : {date}", date.ToString());

            var results = db.Query<UserCollectionsRuns>(query);
            return results;
        }

        public IEnumerable<DataCollectionsRuns> GetDataCollectionsRuns(DateTime date)
        {
            var db = GetRelativityProvidersDatabase();
            var query = string.Format("SELECT [ID],[ModifiedTime] FROM {0} where modifiedTime > '{1}'", DataCollectionRunsTableName, date);

            log.Debug("BillingValidator: Querying DataCollectionRuns for Date : {date}", date.ToString());

            var results = db.Query<DataCollectionsRuns>(query);
            return results;
        }      
    }
}
