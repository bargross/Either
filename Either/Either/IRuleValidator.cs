using System;
using System.Linq.Expressions;

namespace Either
{
    public interface IRuleValidator<L, R>
    {
        bool ValidateRuleFor(L left);
        bool ValidateRuleFor(R right);

        Rule<T> Pack<T>(string ruleName, Expression<Func<T, bool>> rule);
    }
}