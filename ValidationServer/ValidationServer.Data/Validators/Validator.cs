using PrecisionDiscovery.Diagnostics;
using System.Collections.Generic;

namespace ValidationServer.Data.Validators
{
    public class Validator
    {
        protected static PrecisionDiscovery.Diagnostics.Logging.IPDLogger log = PrecisionDiscovery.Diagnostics.Logging.PDLogManager.GetCurrentClassLogger();

        private List<string> _tags = new List<string>();

        public IList<string> FilterSequence
        {
            get
            {
                return this._tags;
            }
        }

        protected string ValidatorProxyId { get; set; }

        public Validator(string validatorProxyId)
        {
            this.ValidatorProxyId = Guard.NotNullOrEmpty(validatorProxyId, "validatorProxyId", log);

            this._tags.Add(this.ValidatorProxyId);
        }

        protected void AddTag(string tag)
        {
            var newTag = Guard.NotNullOrEmpty(tag, "tag", log);
            this._tags.Add(newTag);
        }
    }
}