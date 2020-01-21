using System;
using System.Linq;

using Either.Root;
using Either.Rule;
using Either.Extensions;
using Either.Exceptions;

namespace Either
{
    public class Either<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private RootEither<TLeft, TRight> _root;
        private static RuleValidator<TLeft, TRight> _ruleValidator;

        private readonly Type _currentType;
        private readonly bool _isLeft;
        private bool _disposed = false;

        public Either() => _ruleValidator = new RuleValidator<TLeft, TRight>();

        public Either(TLeft left)
        {
            _root = left;

            _ruleValidator = new RuleValidator<TLeft, TRight>();

            _currentType = typeof(TLeft);

            _isLeft = true;
        }

        public Either(TRight right)
        {
            _root = right;

            _ruleValidator = new RuleValidator<TLeft, TRight>();

            _currentType = typeof(TRight);

            _isLeft = false;
        }

        ~Either() => Dispose(false);

        public void AddRule(string ruleName, Func<TLeft, bool> rule)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }

            Rule<TLeft> packedRule = RuleValidationExtension.Pack(ruleName, rule);

            _ruleValidator.AddRule(packedRule);
        }

        public void AddRule(string ruleName, Func<TRight, bool> rule)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }

            Rule<TRight> packedRule = RuleValidationExtension.Pack(ruleName, rule);

            _ruleValidator.AddRule(packedRule);
        }

        public void ReplaceRule(string ruleName, Func<TLeft, bool> replacement)
        {
            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }
            
            _ruleValidator.Replace(ruleName, replacement);
        }

        public void ReplaceRule(string ruleName, Func<TRight, bool> replacement)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }
            
            _ruleValidator.Replace(ruleName, replacement);
        }

        public bool IsLeftValid() => _ruleValidator.ValidateRuleFor(_root.Left);
        public bool IsRightValid() => _ruleValidator.ValidateRuleFor(_root.Right);

        public T GetValue<T>()
        {
            var type = typeof(T);
            
            if(_currentType == type)
            {
                if(_isLeft) 
                {
                    if(!IsLeftValid())
                    {
                        throw new RuleValidationException(string.Join("/r", _ruleValidator.FailedValidationMessages));
                    }
                    
                    return (T)Convert.ChangeType(_root.Left, type);
                } 
           
                if(!IsRightValid())
                {
                    throw new RuleValidationException(string.Join("/r", _ruleValidator.FailedValidationMessages));
                }

                return (T)Convert.ChangeType(_root.Right, type);
            }

            throw new InvalidCastException($"Either {typeof(TLeft)} nor {typeof(TRight)} match type: {typeof(T)}");
        }

        public void ResetRulesForLeft() => _ruleValidator.ResetRulesForLeft();
        public void ResetRulesForRight() => _ruleValidator.ResetRulesForRight();

        public void ResetRules()
        {
            ResetRulesForLeft();
            ResetRulesForRight();
        }

        public void SetValidatorOptions(Action<IRuleValidator<TLeft, TRight>> setOptions) => setOptions.Invoke(_ruleValidator);

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
                    _ruleValidator.Dispose();
                }

                _disposed = true;
            }
        }

        internal void SetRules(RuleValidator<TLeft, TRight> rules) => _ruleValidator = rules;

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            var newInstance = new Either<TLeft, TRight>(right);
            newInstance.SetRules(_ruleValidator);

            return newInstance;
        }

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        { 
            var newInstance = new Either<TLeft, TRight>(left);
            newInstance.SetRules(_ruleValidator);

            return newInstance;
        }

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();
    }

}
