using BillingValidator.Models.Relativity;
using BillingValidator.Repositories.RelativityRepos;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace BillingValidator.Validators
{
    public class BillingRanValidator : Validator, IValidator
    {
        private IRelativityBillingRepo _relBillingRepo;
        private IRelativityProvidersRepo _relProvidersRepo;
        private BillingValidatorSettings _billingSettings;

        private const string CaseStatisticsMonthlyTableUpdated = "RelativityBillingCaseStatisticsMonthlyTableUpdated";
        private const string CaseStatisticsTableUpdated = "RelativityBillingCaseStatisticsTableUpdated";
        private const string DataCollectionRunsTableUpdated = "RelativityProviderDataCollectionRunsTableUpdated";
        private const string UserCollectionRunsTableUpdated = "RelativityProviderUserCollectionRunsTableUpdated";

        public const string BillingSettingsConstructorParameterName = "billingSettings";

        public BillingRanValidator(IRelativityBillingRepo relBillingRepo, IRelativityProvidersRepo relProvidersRepo, BillingValidatorSettings billingSettings)
            : base(BillingValidatorProxy.BillingValidatorId)
        {
            this._relBillingRepo = Guard.NotNull(relBillingRepo, "relBillingRepo", log);
            this._relProvidersRepo = Guard.NotNull(relProvidersRepo, "relProvidersRepo", log);
            this._billingSettings = Guard.NotNull(billingSettings, "billingSettings", log);
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Debug("BillingValidator: BillingValidator beginning validation.");

            List<IValidatorResult> validatorResults = new List<IValidatorResult>();

            validatorResults.Add(this.RelativityBillingCaseStatisticsMonthlyTableUpdated(context));
            validatorResults.Add(this.RelativityBillingCaseStatisticsTableUpdated(context));
            validatorResults.Add(this.RelativityProviderDataCollectionRunsTableUpdated(context));
            validatorResults.Add(this.RelativityProviderUserCollectionRunsTableUpdated(context));

            return validatorResults;
        }

        private IValidatorResult RelativityBillingCaseStatisticsMonthlyTableUpdated(IValidatorContext validatorContext)
        {
            log.Debug("BillingValidator: Running {testName} billing check.", CaseStatisticsMonthlyTableUpdated);

            return CheckRelativityDbValueNotUpdated(validatorContext,
                                            CaseStatisticsMonthlyTableUpdated,
                                            () =>
                                            {
                                                DateTime dateToCheck = this.GetBillingRunDate(this._billingSettings.BillingRunTime);
                                                CaseStatisticsMonthly caseStatisticsMonthly = _relBillingRepo.GetCaseStatisticsMonthly(dateToCheck.Date).FirstOrDefault();
                                                return caseStatisticsMonthly == null;
                                            });
        }

        private IValidatorResult RelativityBillingCaseStatisticsTableUpdated(IValidatorContext validatorContext)
        {
            log.Debug("BillingValidator: Running {testName} billing check.", CaseStatisticsTableUpdated);

            return CheckRelativityDbValueNotUpdated(validatorContext,
                                            CaseStatisticsTableUpdated,
                                            () =>
                                            {
                                                DateTime dateToCheck = this.GetBillingRunDate(this._billingSettings.BillingRunTime);
                                                CaseStatistics caseStatistics = _relBillingRepo.GetCaseStatistics(dateToCheck.Date).FirstOrDefault();
                                                return caseStatistics == null;
                                            });
        }

        private IValidatorResult RelativityProviderDataCollectionRunsTableUpdated(IValidatorContext validatorContext)
        {
            log.Debug("BillingValidator: Running {testName} billing check.", DataCollectionRunsTableUpdated);

            return CheckRelativityDbValueNotUpdated(validatorContext,
                                            DataCollectionRunsTableUpdated,
                                            () =>
                                            {
                                                DateTime dateToCheck = this.GetBillingRunDate(this._billingSettings.BillingRunTime);
                                                DataCollectionsRuns dataCollectionRun = this._relProvidersRepo.GetDataCollectionsRuns(dateToCheck.Date).FirstOrDefault();
                                                return dataCollectionRun == null;
                                            });
        }

        private IValidatorResult RelativityProviderUserCollectionRunsTableUpdated(IValidatorContext validatorContext)
        {
            log.Debug("BillingValidator: Running {testName} billing check.", UserCollectionRunsTableUpdated);

            return CheckRelativityDbValueNotUpdated(validatorContext,
                                           UserCollectionRunsTableUpdated,
                                           () =>
                                               {
                                                   DateTime dateToCheck = this.GetBillingRunDate(this._billingSettings.BillingRunTime);
                                                   UserCollectionsRuns userCollectionRun = this._relProvidersRepo.GetUserCollectionsRuns(dateToCheck.Date).FirstOrDefault();
                                                   return userCollectionRun == null;
                                               });
        }

        private IValidatorResult CheckRelativityDbValueNotUpdated(IValidatorContext validatorContext, 
                                        string dbCheckDescription, Func<bool> valueDoesNotExistInDb)
        {
            ValidatorResultCode code = ValidatorResultCode.Success;
            string description = string.Format("{0} was updated", dbCheckDescription);

            try
            {
                if (valueDoesNotExistInDb())
                {
                    code = ValidatorResultCode.Error;
                    description = string.Format("{0} was not updated", dbCheckDescription);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, string.Format("{0}{1}", "BillingValidator: ",ex.Message));
                code = ValidatorResultCode.Error;
                description = string.Format("{0} was not updated", dbCheckDescription);
            }

            return new ValidatorResult(this.ValidatorProxyId, description, code, dbCheckDescription);
        }

        private DateTime GetBillingRunDate(int? billingRunTime)
        {
            var dateToCheck = DateTime.Now;

            if (billingRunTime.HasValue && dateToCheck.Hour < billingRunTime.Value)
            {
                dateToCheck = dateToCheck.AddDays(-1);
            }

            log.Debug("BillingValidator: BillingRunTime was determined to be {time}", dateToCheck.ToString());

            return dateToCheck.Date;
        }
    }
}