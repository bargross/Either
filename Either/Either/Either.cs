using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Either
{
    public class Either<L, R> : IRule<L, R>
    {
        private RootEither<L, R> _root;
        private RuleValidator<L, R> _rules;

        private bool _initialized;

        public Either()
        {
            _rules = new RuleValidator<L, R>();
        }

        public Either(L left)
        {
            _root = left;
            
            _rules = new RuleValidator<L, R>();
        }

        public Either(R right)
        {
            _root = right;
            
            _rules = new RuleValidator<L, R>();
        }

        public void AddRule(string ruleName, Expression<Func<L, bool>> rule) => _rules.AddRule(_rules.Pack<L>(ruleName, rule));
        public void AddRule(string ruleName, Expression<Func<R, bool>> rule) => _rules.AddRule(_rules.Pack<R>(ruleName, rule));

        public bool IsLeftValid() => _rules.ValidateRuleFor(_root.Left);
        public bool IsRightValid() => _rules.ValidateRuleFor(_root.Right);

        // public L GetLeft()
        // {
        //     if(IsLeftValid())
        //     {
        //         return Left
        //     }
        // }

        // public R GetRight()
        // {

        // }
    }
}
