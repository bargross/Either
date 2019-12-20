using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Either
{
    public class Either<L, R> : IRule<L, R>
    {
        private RootEither<L, R> root;

        private Dictionary<string, Expression<Func<L, R, bool>>> _rules;

        public Either()
        {
            _rules = new Dictionary<string, Expression<Func<L, R, bool>>>();
        }

        public Either(L left)
        {
            root = left;
        }

        public Either(R right)
        {
            root = right;
        }

        public void AddRule(string ruleName, Expression<Func<L, R, bool>> rule)
        {
            throw new NotImplementedException();
        }

        public void AddRule(string ruleName, Expression<Func<L, bool>> rule)
        {
            throw new NotImplementedException();
        }

        public void AddRule(string ruleName, Expression<Func<R, bool>> rule)
        {
            throw new NotImplementedException();
        }

        public void AddRules(string ruleName, Expression<Func<L, R, bool>>[] rules)
        {
            throw new NotImplementedException();
        }

        private void ValidateRules()
        {
            throw new NotImplementedException();
        }
    }
}
