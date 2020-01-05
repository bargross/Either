using System;

using Either.Root;
using Either.Rule;
using Either.Extensions;
using Either.Exceptions;

namespace Either
{
    public class Either<TLeft, TRight> : IEither<TLeft, TRight>, IDisposable
    {
        private RootEither<TLeft, TRight> _root;
        private static RuleValidator<TLeft, TRight> _rules;

        private readonly Type _currentType;
        private readonly bool _isLeft;
        private bool _disposed = false;

        public bool ValidatorInstantiated { get; private set; }

        public Either(RuleValidator<TLeft, TRight> validator=null) {
            _rules = validator != null ? validator : new RuleValidator<TLeft, TRight>();
            ValidatorInstantiated = true;
        }

        public Either(TLeft left, RuleValidator<TLeft, TRight> validator=null)
        {
            _root = left;
            
            InitializeValidator(_rules, validator);
            ValidatorInstantiated = true;

            _currentType = typeof(TLeft);

            _isLeft = true;
        }

        public Either(TRight right, RuleValidator<TLeft, TRight> validator=null)
        {
            _root = right;
            
            InitializeValidator(_rules, validator);
            ValidatorInstantiated = true;

            _currentType = typeof(TRight);

            _isLeft = false;
        }

        ~Either() => Dispose(false);

        public void AddRule(string ruleName, Func<TLeft, bool> rule) {
            Rule<TLeft> packedRule = RuleValidationExtension.Pack(ruleName, rule);
            
            if(packedRule != null)
            {
                _rules.AddRule(packedRule);
            }
        } 
        public void AddRule(string ruleName, Func<TRight, bool> rule)
        {
            Rule<TRight> packedRule = RuleValidationExtension.Pack(ruleName, rule);
            
            if(packedRule != null)
            {
                _rules.AddRule(packedRule);
            }
        }

        public void ReplaceRule(string ruleName, Func<TLeft, bool> replacement) => _rules.Replace(ruleName, replacement);
        public void ReplaceRule(string ruleName, Func<TRight, bool> replacement) => _rules.Replace(ruleName, replacement);

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

        public void ResetRulesForLeft() => _rules.ResetRulesForLeft();
        public void ResetRulesForRight() => _rules.ResetRulesForRight();

        public void ResetRules()
        {
            ResetRulesForLeft();
            ResetRulesForRight();
        }

        private void InitializeValidator(RuleValidator<TLeft, TRight> validatorAtScope, RuleValidator<TLeft, TRight> replacement = null)
        {
            bool instantiated = false;
            if(validatorAtScope == null) 
            {
                validatorAtScope = new RuleValidator<TLeft, TRight>();
                instantiated = true;
            }
            
            if(!instantiated && replacement != null)
            {
                validatorAtScope = replacement;
            }
        }

        // IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if (disposing) 
                {
                    _rules.Dispose();
                }

                _disposed = true;
            }
        }

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(right, _rules);
        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left, _rules);

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();
    }

}
