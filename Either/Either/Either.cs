using System;
using System.Linq;

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

        public Either() => _rules = new RuleValidator<TLeft, TRight>();

        public Either(TLeft left)
        {
            _root = left;

            _rules = new RuleValidator<TLeft, TRight>();

            _currentType = typeof(TLeft);

            _isLeft = true;
        }

        public Either(TRight right)
        {
            _root = right;

            _rules = new RuleValidator<TLeft, TRight>();

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

            _rules.AddRule(packedRule);
        }

        public void AddRule(string ruleName, Func<TRight, bool> rule)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }

            Rule<TRight> packedRule = RuleValidationExtension.Pack(ruleName, rule);

            _rules.AddRule(packedRule);
        }

        public void ReplaceRule(string ruleName, Func<TLeft, bool> replacement)
        {
            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }
            
            _rules.Replace(ruleName, replacement);
        }

        public void ReplaceRule(string ruleName, Func<TRight, bool> replacement)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }
            
            _rules.Replace(ruleName, replacement);
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
                        throw new RuleValidationException(string.Join("/r", _rules.FailedValidationMessages));
                    }
                    
                    return (T)Convert.ChangeType(_root.Left, type);
                } 
           
                if(!IsRightValid())
                {
                    throw new RuleValidationException(string.Join("/r", _rules.FailedValidationMessages));
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

        internal void SetRules(RuleValidator<TLeft, TRight> rules) => _rules = rules;

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            var newInstance = new Either<TLeft, TRight>(right);
            newInstance.SetRules(_rules);

            return newInstance;
        }

        public static implicit operator Either<TLeft, TRight>(TLeft left)
        { 
            var newInstance = new Either<TLeft, TRight>(left);
            newInstance.SetRules(_rules);

            return newInstance;
        }

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();
    }

}
