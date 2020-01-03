using System;
using System.Linq.Expressions;

namespace Either
{
    public interface IEither<TLeft, TRight>
    {
        T GetValue<T>();
        bool IsLeftValid();
        bool IsRightValid();
        void AddRule(string ruleName, Expression<Func<TLeft, bool>> rule);
        void AddRule(string ruleName, Expression<Func<TRight, bool>> rule);
    }
}
