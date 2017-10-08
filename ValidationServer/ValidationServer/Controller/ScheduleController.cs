using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ValidationServer.Data.Scheduler;
using ValidationServer.Interfaces;

namespace ValidationServer.Controller
{
    public class ScheduleController : ApiController
    {
        /* TODO External API
           Provide list of everything thats available and their schedule 
           CRUD ability for the schedules */

        private IValidationScheduler _validationScheduler;

        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        public ScheduleController(IValidationScheduler validationScheduler)
        {
            this._validationScheduler = Guard.NotNull(validationScheduler, "validationScheduler", log);
        }

        public HttpResponseMessage Get()
        {
            log.Info("ScheduleController get all schedules");

            IEnumerable <IScheduledValidation> validations = null;

            try
            {
                validations = _validationScheduler.GetScheduledValidations();

            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Exception in ScheduleController get schedules");
                return BuildErrorResponse(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, validations);
        }

        [HttpPut]
        public HttpResponseMessage Trigger(ScheduleItemRequest request)
        {
            log.Info("ScheduleController trigger schedule");

            try
            {
                Guard.NotNull(request, "request", log);

                this._validationScheduler.TriggerJob(request.ValidationId, request.ValidationInstanceId);
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Exception in ScheduleController trigger schedule");
                return BuildErrorResponse(ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private HttpResponseMessage BuildErrorResponse(Exception ex)
        {
            var response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            response.ReasonPhrase = ex.Message;
            return response;
        }

        /* TODO:        
            work out logic next sprint          
            void TriggerValidator(string ValidatorId, string validatorInstanceId);

            public void TriggerJob(string jobId)
            {
                Guard.NotNullOrEmpty(jobId, "jobId", log);

                RecurringJob.Trigger(jobId);
            } 

            when UI availabile add sort order
            For GetScheduledValidations - pass in long page, long pageSize then call HangFireScheduler
            IEnumerable<IScheduledValidation> GetScheduledValidations(int startingFrom, int endingAt);

            public void TriggerValidator(string validatorId, string validatorInstanceId)
            {
                //TODO - Trigger fails if jobid is null, but not if the job id does not exist.
                //Need to handle it
                var jobId = this.GetJobId(validatorInstanceId, validatorInstanceId);
                this._validationScheduleRepository.TriggerJob(jobId);
            }
                
            IList<ISchedulableValidator> GetValidatorsAvailableToBeScheduled();
        */
    }
}