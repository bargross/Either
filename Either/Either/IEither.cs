using System;
using System.Linq.Expressions;

namespace Either
{
    public interface IEither<L, R>
    {
        T GetValue<T>();
        bool IsLeftValid();
        bool IsRightValid();
        void AddRule(string ruleName, Expression<Func<L, bool>> rule);
        void AddRule(string ruleName, Expression<Func<R, bool>> rule);
    }
}
