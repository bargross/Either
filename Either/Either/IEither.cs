using System;
using System.Linq.Expressions;

namespace Either
{
    public interface IEither<TLeft, TRight>
    {
        T GetValue<T>();
        bool IsLeftValid();
        bool IsRightValid();
        void AddRule(string ruleName, Func<TLeft, bool> rule);
        void AddRule(string ruleName, Func<TRight, bool> rule);
        void ResetRulesForLeft();
        void ResetRulesForRight();
        void ResetRules();
    }
}
