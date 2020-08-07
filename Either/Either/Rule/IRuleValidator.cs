using System;
using System.Collections.Generic;

namespace Either.Rule
{
    public interface IRuleValidator<TLeft, TRight> : IDisposable
    {
        bool TerminateOnFail { get; set; }
        bool IsLeftValue { get; set; }
        int FailedCount { get; }
        int RuleCount { get; }
        IList<string> FailedValidationMessages { get; }

        IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TLeft, bool> rule);
        IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TRight, bool> rule);
        IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TLeft, bool> replacement);
        IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TRight, bool> replacement);

        bool ContainsRule(string ruleName);
        bool ValidateRuleFor(TLeft left);
        bool ValidateRuleFor(TRight right);
        void RemoveRulesForLeft();
        void RemoveRulesForRight();
        void ResetRulesForLeft();
        void ResetRulesForRight();
        bool GetRuleValidationResult(string ruleName);
    }
}