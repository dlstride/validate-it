using HareDu;
using HareDu.Model;
using HareDu.Resources;
using PrecisionDiscovery.Diagnostics;
using RabbitMQValidator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationServer.Data.Enumerations;
using ValidationServer.Data.Validators;

namespace RabbitMQValidator.Validators
{
    public class RabbitMQErrorValidator : Validator, IValidator
    {
        public const string RabbitMQValidatorSettingsParamName = "rabbitMQValidatorSettings";

        private const string RabbitMQResultIdentifier = "RabbitMQValidator";

        private RabbitMQValidatorSettings _rabbitMQValidatorSettings;

        public RabbitMQErrorValidator(RabbitMQValidatorSettings rabbitMQValidatorSettings) :
            base(RabbitMQValidatorProxy.RabbitMQValidatorId)
        {
            this._rabbitMQValidatorSettings = Guard.NotNull(rabbitMQValidatorSettings, "rabbitMQValidatorSettings", log);
            this.AddTags();
        }

        public IList<IValidatorResult> Execute(IValidatorContext context)
        {
            log.Info("RabbitMQValidator Execute started");

            List<IValidatorResult> results = new List<IValidatorResult>();

            log.Debug("Validating rabbit mq environment for Server: {Server} Port: {Port} vHost: {vHost}", 
                        _rabbitMQValidatorSettings.HostServer, _rabbitMQValidatorSettings.Port, _rabbitMQValidatorSettings.vHost);

            HareDuSettings settings =  new HareDuSettings()
                            {
                                RabbitUrl = string.Format("http://{0}:{1}", _rabbitMQValidatorSettings.HostServer, _rabbitMQValidatorSettings.Port),
                                Vhost = _rabbitMQValidatorSettings.vHost,
                                Username = _rabbitMQValidatorSettings.UserName,
                                Password = _rabbitMQValidatorSettings.Password
                            };

            var queues = this.GetErrorQueues(settings);

            foreach (var queue in queues)
            {
                log.Debug("RabbitMQValidator validating queue {RabbitQueue}", queue.Name );
                var queueResult = this.GetQueueResult(settings, queue);
                results.Add(queueResult);
            }

            log.Info("Starting RabbitMQValidator Execute completed with {ResultCount} results", results.Count);

            return results;
        }

        private void AddTags()
        {
            var server = this._rabbitMQValidatorSettings.HostServer.Replace(".pd.local", "").Replace(".pddev.local", "");

            log.Debug("RabbitMQValidator: adding Tag {Server}", server);
            log.Debug("RabbitMQValidator: adding Tag {vHost}", _rabbitMQValidatorSettings.vHost);

            this.AddTag(server);
            this.AddTag(this._rabbitMQValidatorSettings.vHost);
        }

        private IValidatorResult GetQueueResult(Models.HareDuSettings settings, RabbitQueue queue)
        {
            log.Debug("RabbitMQValidator: GetQueueResult for {QueueName} ", queue.Name);

            var tags = new List<string>() { queue.Name };

            string resultIdentifier = string.Format("{0}:{1}", queue.Name, queue.MessagesReady);

            var messageCount = string.IsNullOrEmpty(queue.Messages) ? "0" : queue.Messages;

            var description = string.Format("Server: {0}{4}             vHost: {1}{5}             {2} Messages found in error queue {3}",
                         settings.RabbitUrl, settings.Vhost, messageCount, queue.Name, Environment.NewLine, Environment.NewLine);

            ValidatorResultCode code = ValidatorResultCode.Success;

            if (queue.Messages != null && queue.MessagesReady > 0)
            {
                code = ValidatorResultCode.Error;
            }

            log.Debug("RabbitMQValidator GetQueueResult for {QueueName} has messages {Message} and message ready count {MessagesReady}", queue.Name, queue.Messages, queue.MessagesReady);

            return new ValidatorResult(this.ValidatorProxyId, description, code, queue.Name, tags);
        }

        private IEnumerable<RabbitQueue> GetErrorQueues(Models.HareDuSettings settings)
        {
            HareDuClient client = HareDuFactory.New(x => x.ConnectTo(settings.RabbitUrl));
            VirtualHostResources res = client.Factory<VirtualHostResources>(x => x.Credentials(settings.Username, settings.Password));
            IEnumerable<Queue> queues = res.Queue.GetAll(target => target.VirtualHost(settings.Vhost)).Data().ToList();

            IEnumerable<RabbitQueue> results = queues.Where(q => q.Name.EndsWith("_error")).Select(x =>
                          new RabbitQueue(x.Name, x.Messages, x.MessagesReady) { }).ToList();

            return results;
        }
    }
}