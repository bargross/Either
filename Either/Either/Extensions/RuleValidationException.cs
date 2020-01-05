using System;

using Either.Rule;

namespace Either.Extensions
{
    public static class RuleValidationExtension
    {
        public static Rule<T> Pack<T>(string ruleName, Func<T, bool> rule)
        {
            return new Rule<T> {
                RuleName = ruleName,
                TypeRule = rule
            };
        }
    }
}