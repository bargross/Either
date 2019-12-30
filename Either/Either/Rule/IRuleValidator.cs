using System;
using System.Linq.Expressions;

namespace Either.Rule
{
    public interface IRuleValidator<L, R>
    {
        bool TerminateOnFail { get; set; }
        void AddRule(Rule<R> rule);
        void AddRule(Rule<L> rule);
        bool ValidateRuleFor(L left);
        bool ValidateRuleFor(R right);

        Rule<T> Pack<T>(string ruleName, Expression<Func<T, bool>> rule);
    }
}