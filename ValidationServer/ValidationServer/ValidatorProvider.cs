using Ninject;
using Ninject.Extensions.ChildKernel;
using PrecisionDiscovery.Configuration.Vasa;
using PrecisionDiscovery.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationServer.Bootstrap;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;
using ValidationServer.Interfaces;

namespace ValidationServer
{
    public class ValidatorProvider : IValidatorProvider
    {
        private static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        private IKernel _kernel;
        private IChildKernelCreator _childCreator;
        private IVasaClient _vsVasa;
        private IEnumerable<IValidatorProxy> _validatorProxies;
        private IValidatorModuleConfig _validatorModuleConfig;

        private const string NoValidatorInstancesFound = "No instances found";
        private const string FailedToLoadModule = "Failed to create child kernel for module {0}";

        public ValidatorProvider(IKernel kernel, IChildKernelCreator childCreator, IVasaClient vsVasa, IValidatorModuleConfig validatorModuleConfig)
        {
            this._kernel = Guard.NotNull(kernel, "kernel", log);
            this._childCreator = Guard.NotNull(childCreator, "childKernel", log);
            this._vsVasa = Guard.NotNull(vsVasa, "vsVasa", log);

            this._validatorModuleConfig = Guard.NotNull(validatorModuleConfig, "validatorModuleConfig", log);

            this.DiscoverValidatorProxies();
            this.DiscoverValidatorInstances();
        }


        public IEnumerable<IValidatorProxy> ValidatorProxies
        {
            get
            {
                return this._validatorProxies;
            }
        }

        public IValidatorProxy GetValidatorProxy(string validatorId)
        {
            var proxy = this._validatorProxies.SingleOrDefault(px => px.ValidatorId == validatorId);

            if (proxy == null)
                throw new Exception(string.Format("ScheduleRequest with ValidatorId: {0} was invalid. No Proxy found with that Id", validatorId));

            return proxy;
        }

        public IValidatorInstance GetValidatorInstance(string validationId, string validatorInstanceId)
        {
            Guard.NotNullOrEmpty(validationId, "validationId", log);
            Guard.NotNullOrEmpty(validatorInstanceId, "validatorInstanceId", log);

            IValidatorProxy proxy = this.GetValidatorProxy(validationId);

            var instance = proxy.ValidatorInstances.SingleOrDefault(vi => vi.ValidatorInstanceId == validatorInstanceId);

            if (instance == null)
                throw new Exception(string.Format("ScheduleRequest with ValidatorInstanceId: {0} was invalid. No ValidatorInstance found with that Id", validatorInstanceId));

            return instance;
        }

        private void DiscoverValidatorProxies()
        {
            List<IValidatorProxy> validatorProxies = null;

            log.Debug("Validation Server starting to validate...");

            try
            {
                validatorProxies = this.GetValidatorProxies(_kernel, _childCreator, this._validatorModuleConfig.ModuleNames);

                log.Debug("{ValidatorProxyCount} Validator Proxies discovered and queued for validation.", validatorProxies.Count);
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);

                string error = string.Format("There was an unhandled exception thrown when running the validators. Exception: {0}",
                    ex.ToString());

                throw new Exception(error);
            }

            this._validatorProxies = validatorProxies ?? new List<IValidatorProxy>();
        }

        private void DiscoverValidatorInstances()
        {
            foreach (var validatorProxy in this._validatorProxies)
            {
                List<ConfigSection> configValues = this.GetValidatorConfigSettings(validatorProxy);
                log.Debug("Validator: {ValidatorId}  Version:  {ValidatorVersion}{NewLine}   Description:  {Description}", validatorProxy.ValidatorId, validatorProxy.Version.ToString(), Environment.NewLine, validatorProxy.Description);

                IEnumerable<IValidatorInstance> validatorInstances = validatorProxy.GetAvailableValidators(configValues);

                this.WarnWhenNoValidatorInstancesFound(validatorProxy, validatorInstances.Count());

                //foreach (var validatorInstance in validatorInstances)
                //{
                //    log.Debug("ValidatorInstance: {Name}  Description: {Description} beginning to validate.", validatorInstance.Name, validatorInstance.Description);

                //    List<IValidatorRunEntry> runEntries = this.RunValidator(validatorProxy, validatorInstance);

                //    this.SendValidatorResults(runEntries);
                //}
            }
        }

        private List<ConfigSection> GetValidatorConfigSettings(IValidatorProxy proxy)
        {
            List<ConfigSection> configValues = null;
            try
            {
                configValues = this._vsVasa.GetConfigSections(null, proxy.ValidatorId);
            }
            catch (Exception ex)
            {
                log.Debug("ValidatorProxy: {proxy} had no Vasa configuration settings that were found. Exception {ExceptionMessage}", proxy.ValidatorId, ex.Message);
            }

            return configValues ?? new List<ConfigSection>();
        }

        private void WarnWhenNoValidatorInstancesFound(IValidatorProxy proxy, int validatorInstanceCount)
        {
            if (validatorInstanceCount == 0)
            {
                var message = string.Format("No validators were found for {0}", proxy.ValidatorId);

                var vr = new ValidatorResult(proxy.ValidatorId, message, ValidatorResultCode.Warning, NoValidatorInstancesFound);

                var vre = new ValidatorRunEntry(DateTime.Now, DateTime.Now, vr, new List<string>() { proxy.ValidatorId }) { } as IValidatorRunEntry;

                //TODO 
                // change error ... throw instead
                //this.SendValidatorResults(new List<IValidatorRunEntry>() { vre });
            }
        }

        private List<IValidatorProxy> GetValidatorProxies(IKernel kernel, IChildKernelCreator childCreator, List<string> modules)
        {
            List<IValidatorProxy> validatorProxies = new List<IValidatorProxy>();

            List<ChildKernel> childKernels = LoadValidators(kernel, childCreator, modules);

            log.Debug("There were {childKernelCount} childKernels discovered.", childKernels.Count);

            foreach (var child in childKernels)
            {
                validatorProxies.AddRange(child.GetAll<IValidatorProxy>());
            }

            return validatorProxies;
        }

        private List<ChildKernel> LoadValidators(IKernel kernel, IChildKernelCreator childCreator, List<string> modules)
        {
            // discover dlls which contain validators
            // each validator gets a childkernel for their own separate kernels included in their dlls.

            List<ChildKernel> childKernels = new List<ChildKernel>();

            //Shared names and conventions 
            modules.ForEach(m =>
            {
                try
                {
                    var childKernel = childCreator.CreateChildKernelWithModule(kernel, m);

                    if (childKernel == null)
                    {
                        var msg = string.Format(FailedToLoadModule, m);
                        log.Error(string.Format(msg));
                        throw new Exception(msg);
                    }

                    childKernels.Add(childKernel);
                }
                catch (Exception ex)
                {
                    log.Error("An unhandled exception occurred loading validators: {0}", ex);
                    throw ex;
                }
            });
            return childKernels;
        }
    }
}