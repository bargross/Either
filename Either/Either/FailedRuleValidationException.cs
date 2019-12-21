using System;

namespace Either
{
    internal class RuleFailedException : Exception
    {
        public RuleFailedException()
        {
        }

        public RuleFailedException(string message)
            : base(message)
        {
        }

        public RuleFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}