using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;
using ValidationServer.Interfaces;
using ValidationServer.Scheduler;
using ValidationServer.Scheduler.Scheduler;

namespace ValidationServer
{
    public delegate void ReturnValidatorResults(object sender, ValidatorResultsArg e);

    public class ValidatorRunner : IValidatorRunner, IDisposable
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        private IValidatorProvider _validatorProvider;

        private const string UnhandledExceptionResultCode = "Unhandled Exception";

        private BlockingCollection<ValidatorInstanceInfo> _validatorsToRun;

        public event ReturnValidatorResults ValidatorResultsReady;

#if DEBUG
        public static int JobsQueued = 0;

        public static int JobsRan = 0;
#endif

        public ValidatorRunner(IValidatorProvider validatorProvider)
        {
            _validatorProvider = Guard.NotNull(validatorProvider, "validatorProvider", log);
            _validatorsToRun = new BlockingCollection<ValidatorInstanceInfo>();
            InitializeConsumers();
        }

        private void InitializeConsumers()
        {
#if DEBUG
            if (!HangFireScheduler._stopWatch.IsRunning) HangFireScheduler._stopWatch.Start();
#endif

            Task.Run(() =>
            {
                Parallel.ForEach(_validatorsToRun.GetConsumingPartitioner(), validatorInfo =>
                {
                    IList<IValidatorRunEntry> results = this.RunValidatorInstance(validatorInfo);
                    ValidatorResultsReady.Invoke(this, new ValidatorResultsArg(results));
#if DEBUG
                    var timeElapsed = HangFireScheduler._stopWatch.Elapsed;
                    JobsRan++;
                    Console.WriteLine(string.Format("Time Elapsed {5}     Handled by Thread {4}   - Jobs Queued: {0}   Jobs Ran {1}    Jobs Scheduled: {2}    Jobs Triggered: {3}", JobsQueued, JobsRan, HangFireScheduler.JobsScheduled, HangFireScheduler.JobsScheduledCalledBack, Thread.CurrentThread.ManagedThreadId, timeElapsed));
#endif

                })
                ;
            });
        }

        public void QueueValidatorForExecution(ValidatorInstanceInfo info)
        {
            _validatorsToRun.Add(info);
#if DEBUG
            JobsQueued++;
#endif
        }

        private List<IValidatorRunEntry> RunValidator(ValidatorInstanceInfo info)
        {
            List<IValidatorRunEntry> runEntry = this.RunValidatorInstance(info);
            return runEntry;
        }

        private List<IValidatorRunEntry> RunValidatorInstance(ValidatorInstanceInfo info)
        {
            var startTime = DateTime.Now;
            IValidatorProxy validatorProxy = null;
            try
            {
                validatorProxy = _validatorProvider.GetValidatorProxy(info.ValidatorId);
                IValidatorInstance validatorInstance = _validatorProvider.GetValidatorInstance(info.ValidatorId, info.ValidatorInstanceId);

                log.Debug("Validator: {ValidatorId}  Version:  {ValidatorVersion}{NewLine}   Description:  {Description}", validatorProxy.ValidatorId, validatorProxy.Version.ToString(), Environment.NewLine, validatorProxy.Description);
                log.Debug("ValidatorInstance: {Name}  Description: {Description} beginning to validate.", validatorInstance.Name, validatorInstance.Description);

                IValidator validator = validatorProxy.Create(validatorInstance);
                IValidatorContext vc = new ValidatorContext();
                IList<IValidatorResult> results = validator.Execute(vc);

                var endTime = DateTime.Now;

                log.Debug("ValidatorInstance: {Name} finished validating. Start: {start}  End: {end}", validatorInstance.Name, startTime.ToString(), endTime.ToString());
                log.Debug("ValidatorInstance: {Name} returned {resultCount} results.", validatorInstance.Name, results.Count);

                List<IValidatorRunEntry> runEntries = results.Select(x =>
                                new ValidatorRunEntry(startTime, endTime, x, validator.FilterSequence) { } as IValidatorRunEntry).ToList();

                return runEntries;
            }
            catch (Exception ex)
            {
                log.Error(ex, "{ValidatorProxy} execution caused Error: {Message}", validatorProxy.ValidatorId, ex.Message);

                var vr = new ValidatorResult(validatorProxy.ValidatorId, ex.Message, ValidatorResultCode.Error, UnhandledExceptionResultCode);

                var vre = new ValidatorRunEntry(startTime, DateTime.Now, vr);

                return new List<IValidatorRunEntry>() { vre };
            }
        }

        public void Dispose()
        {
            this._validatorsToRun.CompleteAdding();
        }
    }
}
