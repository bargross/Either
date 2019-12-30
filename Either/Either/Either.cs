using System;
using System.Linq.Expressions;

using Either.Root;
using Either.Rule;

namespace Either
{
    public class Either<L, R> : IEither<L, R>
    {
        private RootEither<L, R> _root;
        private RuleValidator<L, R> _rules;
        private bool _initialized;

        private Type _currentType;
        private bool _isLeft;

        public bool VaidatorInstantiated { get; private set; }

        public Either(RuleValidator<L, R> validator=null) {
            _rules = validator != null ? validator : new RuleValidator<L, R>();
            VaidatorInstantiated = true;
        } 

        public Either(L left, RuleValidator<L, R> validator=null)
        {
            _root = left;
            
            _rules = validator != null ? validator : new RuleValidator<L, R>();
            VaidatorInstantiated = true;

            _currentType = typeof(L);

            _isLeft = true;
        }

        public Either(R right, RuleValidator<L, R> validator=null)
        {
            _root = right;
            
            _rules = validator != null ? validator : new RuleValidator<L, R>();
            VaidatorInstantiated = true;

            _currentType = typeof(R);

            _isLeft = false;
        }

        public void AddRule(string ruleName, Expression<Func<L, bool>> rule) {
            Rule<L> packedRule = _rules.Pack<L>(ruleName, rule);
            
            if(packedRule != null)
            {
                _rules.AddRule(packedRule);
            }
        } 
        public void AddRule(string ruleName, Expression<Func<R, bool>> rule)
        {
            Rule<R> packedRule = _rules.Pack<R>(ruleName, rule);
            
            if(packedRule != null)
            {
                _rules.AddRule(packedRule);
            }
        }

        public bool IsLeftValid() => _rules.ValidateRuleFor(_root.Left);
        public bool IsRightValid() => _rules.ValidateRuleFor(_root.Right);

        public T GetValue<T>()
        {
            var type = typeof(T);
            
            if(_currentType == type)
            {
                if(_isLeft) 
                {
                    if(!IsRightValid())
                    {
                        throw new RuleFailedException(_rules.FailedValidationMessages.ToString());
                    }
                    
                    return (T)Convert.ChangeType(_root.Left, type);
                } 
                else 
                {
                    if(!IsRightValid())
                    {
                        throw new RuleFailedException(_rules.FailedValidationMessages.ToString());
                    }

                    return (T)Convert.ChangeType(_root.Right, type); 
                }
            }

            throw new InvalidCastException($"Either {typeof(L)} nor {typeof(R)} match type: {typeof(T)}");
        }
    }
}
