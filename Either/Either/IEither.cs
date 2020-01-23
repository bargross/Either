using System;
using Either.Rule;

namespace Either
{
    public interface IEither<TLeft, TRight> : IDisposable
    {
        public bool IsValid { get; }

        T GetValue<T>();
        bool IsLeftValid();
        bool IsRightValid();
        void ResetRulesForLeft();
        void ResetRulesForRight();
        void ResetRules();
        bool GetValidationResultForRule(string ruleName);
        void SetValidatorOptions(Action<IRuleValidator<TLeft, TRight>> validator);
    }
}
