using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ValidationSchedulerData.Scheduler;
using ValidationServer.Data.Scheduler;

namespace ValidationServer.Scheduler
{
    public class ScheduleInputParser : IScheduleInputParser
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        private const string ValidatorScheduleFilePath = "ValidationSchedule.txt";
        public IEnumerable<IScheduleRequest> ParseAndCreateScheduleRequest()
        {
            List<IScheduleRequest> scheduleRequests = new List<IScheduleRequest>();
            try
            {
                IEnumerable<string> validatorScheduleItems = File.ReadAllLines(ValidatorScheduleFilePath).Where(s => !string.IsNullOrWhiteSpace(s) && !s.StartsWith("#"));
                foreach (var scheduleItem in validatorScheduleItems)
                {
                    var split = scheduleItem.Split(new char[] { ':' });

                    if (split.Length != 3)
                    {
                        throw new Exception(string.Format("Incorrect format for schedule input file! - Format is [ValidatorId:ValidatorInstanceId:CRON]{0}    Format Given: {1}", Environment.NewLine, scheduleItem));
                    }

                    var validatorId = this.GetStringValue(split, 0, "validatorId");
                    var validatorInstanceId = this.GetStringValue(split, 1, "validatorInstanceId");
                    var cronTabExpression = this.GetStringValue(split, 2, "cronTabExpression");

                    if (string.IsNullOrWhiteSpace(validatorId))
                    {
                        throw new Exception();
                    }

                    var request = new ScheduleRequest(validatorId, validatorInstanceId, cronTabExpression);
                    scheduleRequests.Add(request);

                    log.Debug("Parsed ScheduleRequest from input with ValidatorId: {0} ValidatorInstanceId: {1} and CronTabExpression: {2}", request.ValidatorId, request.ValidatorInstanceId, request.CronTabExpression);
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Exception encountered parsing scheduleInput {0}    Exception: {1}", Environment.NewLine, ex.Message));
                throw ex;
            }
            return scheduleRequests;
        }

        private string GetStringValue(string[] split, int index, string name)
        {
            var value = split[index].Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception(string.Format("No value specified for {0}", name));
            }
            return value;
        }
    }
}