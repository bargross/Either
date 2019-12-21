using System;

namespace Either
{
    internal class FailedRuleValidationException : Exception
    {
        public FailedRuleValidationException()
        {
        }

        public FailedRuleValidationException(string message)
            : base(message)
        {
        }

        public FailedRuleValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}