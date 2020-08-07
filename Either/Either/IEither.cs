using System;
using Either.Model;
using Either.Rule;

namespace Either
{
    public interface IEither<TLeft, TRight> : IDisposable
    {
        public bool IsValid { get; }

        T GetValue<T>();
        bool IsLeftValid();
        bool IsRightValid();
        void ResetLeftRulesToDefault();
        void ResetRightRulesToDefault();
        void ResetAllResultsToDefault();
        void RemoveLeftRules();
        void RemoveRightRules();
        void RemoveAllRules();
        bool GetValidationResultFor(string ruleName);
    }
}
