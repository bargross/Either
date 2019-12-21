using System;
using System.Linq.Expressions;

namespace Either
{
    public class Either<L, R> : IEither<L, R>
    {
        private RootEither<L, R> _root;
        private RuleValidator<L, R> _rules;

        private bool _initialized;

        private Type _currentType;
        private bool _isLeft;

        public Either()
        {
            _rules = new RuleValidator<L, R>();
        }

        public Either(L left)
        {
            _root = left;
            
            _rules = new RuleValidator<L, R>();

            _currentType = typeof(L);

            _isLeft = true;
        }

        public Either(R right)
        {
            _root = right;
            
            _rules = new RuleValidator<L, R>();

            _currentType = typeof(R);

            _isLeft = false;
        }

        public void AddRule(string ruleName, Expression<Func<L, bool>> rule) => _rules.AddRule(_rules.Pack<L>(ruleName, rule));
        public void AddRule(string ruleName, Expression<Func<R, bool>> rule) => _rules.AddRule(_rules.Pack<R>(ruleName, rule));

        public bool IsLeftValid() => _rules.ValidateRuleFor(_root.Left);
        public bool IsRightValid() => _rules.ValidateRuleFor(_root.Right);

        public T GetValue<T>()
        {
            var type = typeof(T);
            
            if(_currentType == type)
            {
                if(_isLeft) {
                    if(!IsRightValid())
                    {
                        throw new RuleFailedException(_rules.FailedRuleMessages.ToString());
                    }
                    
                    return (T)Convert.ChangeType(_root.Left, type);
                } 
                else 
                {
                    if(!IsRightValid())
                    {
                        throw new RuleFailedException(_rules.FailedRuleMessages.ToString());
                    }

                    return (T)Convert.ChangeType(_root.Right, type); 
                }
            }

            throw new InvalidCastException($"Either {typeof(L)} nor {typeof(R)} match type: {typeof(T)}");
        }
    }
}
