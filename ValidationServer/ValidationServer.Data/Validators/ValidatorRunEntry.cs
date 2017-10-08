using System;
using System.Collections.Generic;

namespace ValidationServer.Data.Validators
{
    public class ValidatorRunEntry : IValidatorRunEntry
    {
        private DateTime _startTime;
        private DateTime _finishTime;
        private IValidatorResult _result;
        private IList<string> _validatorTags;

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        }

        public DateTime FinishTime
        {
            get
            {
                return _finishTime;
            }
        }

        public IValidatorResult Result
        {
            get
            {
                return _result;
            }
        }

        public IList<string> FilterSequence
        {
            get
            {
                return _validatorTags;
            }
        }

        public ValidatorRunEntry(DateTime startTime, DateTime finishTime, IValidatorResult result, IList<string> validatorTags = null)
        {
            this._startTime = startTime;
            this._finishTime = finishTime;
            this._result = result;
            this._validatorTags = validatorTags ?? new List<string>();
        }

    }
}