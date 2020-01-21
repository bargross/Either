using System;
using System.Linq.Expressions;

namespace Either.Rule
{
    public interface IRuleValidator<TLeft, TRight> : IDisposable
    {
        bool TerminateOnFail { get; set; }
        bool IsLeftValue { get; set; }
        int FailedCount { get; }
        int RuleCount { get; }

        void AddRule(Rule<TRight> rule);
        void AddRule(Rule<TLeft> rule);
        bool ValidateRuleFor(TLeft left);
        bool ValidateRuleFor(TRight right);
        void ResetRulesForLeft();
        void ResetRulesForRight();
        bool GetRuleValidationResult(string ruleName);
    }
}