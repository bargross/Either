using System;
using System.Linq.Expressions;

using Either.Root;
using Either.Rule;
using Either.Extensions;
using Either.Exceptions;

namespace Either
{
    public class Either<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private readonly RootEither<TLeft, TRight> _root;
        private static RuleValidator<TLeft, TRight> _rules;

        private readonly Type _currentType;
        private readonly bool _isLeft;

        public bool ValidatorInstantiated { get; private set; }

        public Either(RuleValidator<TLeft, TRight> validator=null) {
            _rules = validator != null ? validator : new RuleValidator<TLeft, TRight>();
            ValidatorInstantiated = true;
        }

        public Either(TLeft left, RuleValidator<TLeft, TRight> validator=null)
        {
            _root = left;
            
            _rules = validator != null ? validator : new RuleValidator<TLeft, TRight>();
            ValidatorInstantiated = true;

            _currentType = typeof(TLeft);

            _isLeft = true;
        }

        public Either(TRight right, RuleValidator<TLeft, TRight> validator=null)
        {
            _root = right;
            
            _rules = validator != null ? validator : new RuleValidator<TLeft, TRight>();
            ValidatorInstantiated = true;

            _currentType = typeof(TRight);

            _isLeft = false;
        }

        public void AddRule(string ruleName, Expression<Func<TLeft, bool>> rule) {
            Rule<TLeft> packedRule = RuleValidationExtension.Pack(ruleName, rule);
            
            if(packedRule != null)
            {
                _rules.AddRule(packedRule);
            }
        } 
        public void AddRule(string ruleName, Expression<Func<TRight, bool>> rule)
        {
            Rule<TRight> packedRule = RuleValidationExtension.Pack(ruleName, rule);
            
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
                    if(!IsLeftValid())
                    {
                        throw new RuleValidationException(_rules.FailedValidationMessages.ToString());
                    }
                    
                    return (T)Convert.ChangeType(_root.Left, type);
                } 
           
                if(!IsRightValid())
                {
                    throw new RuleValidationException(_rules.FailedValidationMessages.ToString());
                }

                return (T)Convert.ChangeType(_root.Right, type);
            }

            throw new InvalidCastException($"Either {typeof(TLeft)} nor {typeof(TRight)} match type: {typeof(T)}");
        }

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(right, _rules);
        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left, _rules);

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();
    }

}
