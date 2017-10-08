using BillingValidator.Models.Relativity;
using BillingValidator.Repositories.RelativityRepos;
using PetaPoco;
using PrecisionDiscovery.Configuration.Vasa;
using PrecisionDiscovery.Data;
using PrecisionDiscovery.Data.SQLServer;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingValidator.Repositories.RelativityReposs
{
    public class RelativityBillingRepo : IRelativityBillingRepo
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();
        private IVasaClient _gpodVasa;
        private const string CaseStatisticsTableName = "[RelativityBillingDB].[dbo].[CaseStatistics]";
        private const string CaseStatisticsMonthlyTableName = "[RelativityBillingDB].[dbo].[CaseStatisticsMonthly]";

        public RelativityBillingRepo(IVasaClient gpodVasa)
        {
            this._gpodVasa = Guard.NotNull(gpodVasa, "gpodVasa", log);
        }

        private Database GetDatabase(IDBConnectionInfo connectionInfo)
        {
            var connectionString = ConnectionStringBuilder.GetConnnectionString(connectionInfo);
            Database db = new Database(connectionString, "System.Data.SqlClient");

            return db;
        }

        private IDBConnectionInfo GetRelativityBillingDatabaseConnectionInfo()
        {
            log.Debug("BillingValidator: Building RelativityBillingDatabaseConnectionInfo using Vasa: GPOD  Section: Billing.Relativity");
            var section = _gpodVasa.GetConfigSections(null, "Billing.Relativity");
            var connInfo = section.First().GetAsDBConnectionInfo();
            return connInfo;
        }

        public Database GetRelativityBillingDatabase()
        {
            var connectionInfo = GetRelativityBillingDatabaseConnectionInfo();

            return GetDatabase(connectionInfo);
        }

        public IEnumerable<CaseStatistics> GetCaseStatistics(DateTime date)
        {
            var db = GetRelativityBillingDatabase();
            var query = string.Format("SELECT [ID],[ModifiedTime] FROM {0} where modifiedTime > '{1}'", CaseStatisticsTableName, date);

            log.Debug("BillingValidator: Querying CaseStatisticsTable for Date : {date}", date.ToString());

            var results = db.Query<CaseStatistics>(query);
            return results;
        }

        public IEnumerable<CaseStatisticsMonthly> GetCaseStatisticsMonthly(DateTime date)
        {
            var db = GetRelativityBillingDatabase();
            var query = string.Format("SELECT [ID],[ModifiedTime] FROM {0} where modifiedTime > '{1}'", CaseStatisticsMonthlyTableName, date);

            log.Debug("BillingValidator: Querying CaseStatisticsTableMonthly for Date : {date}", date.ToString());

            var results = db.Query<CaseStatisticsMonthly>(query);
            return results;
        }       
    }
}