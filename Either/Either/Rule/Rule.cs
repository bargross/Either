using System;

namespace Either.Rule
{
    public class Rule<T>
    {
        public string RuleName { get; set; }
        public Func<T, bool> TypeRule { get; set; }
    }
}