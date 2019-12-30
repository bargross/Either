using System;
using System.Linq.Expressions;

namespace Either.Rule
{
    public class Rule<T>
    {
        public string RuleName { get; set; }
        public Expression<Func<T, bool>> TypeRule { get; set; }
    }
}