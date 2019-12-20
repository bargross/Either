using System;
using System.Linq.Expressions;

namespace Either
{
    public interface IRule<L, R>
    {

        void AddRule(string ruleName, Expression<Func<L, bool>> rule);
        void AddRule(string ruleName, Expression<Func<R, bool>> rule);
    }
}
