using System;
using Either.Rule;

namespace Either
{
    public interface IEither<TLeft, TRight> : IDisposable
    {
        T GetValue<T>();
        bool IsLeftValid();
        bool IsRightValid();
        void AddRule(string ruleName, Func<TLeft, bool> rule);
        void AddRule(string ruleName, Func<TRight, bool> rule);
        void ResetRulesForLeft();
        void ResetRulesForRight();
        void ResetRules();

        void SetValidatorOptions(Action<IRuleValidator<TLeft, TRight>> validator);
    }
}
