using System;

using Either.Root;
using Either.Rule;
using Either.Exceptions;

namespace Either
{
    public class Either<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private RootEither<TLeft, TRight> _root;
        private static RuleValidator<TLeft, TRight> _ruleValidator;

        private Type _currentType;
        private bool _isLeft;
        private bool _disposed = false;

        public bool IsValid { get; private set; }
        public bool IsPresent { get; private set;  }

        public Either() => _ruleValidator = new RuleValidator<TLeft, TRight>();

        private Either(TLeft left, RuleValidator<TLeft, TRight> validator)
        {
            _root = left;

            AssignScopeValues(
                _ruleValidator == null ? new RuleValidator<TLeft, TRight>() : validator,
                typeof(TLeft),
                true,
                true,
                IsLeftValid()
            );
        }

        private Either(TRight right, RuleValidator<TLeft, TRight> validator)
        {
            _root = right;

            AssignScopeValues(
                _ruleValidator == null ? new RuleValidator<TLeft, TRight>() : validator,
                typeof(TRight),
                false,
                true,
                IsRightValid()
            );
        }

        ~Either() => Dispose(false);

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

            if (_currentType == type)
            {
                if (!IsValid)
                {
                    throw new RuleValidationException(string.Join("/r", _ruleValidator.FailedValidationMessages));
                }

                if (_isLeft) 
                {
                    return (T)Convert.ChangeType(_root.Left, type);
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

        public bool GetValidationResultForRule(string ruleName) => _ruleValidator.GetRuleValidationResult(ruleName);

        public void SetValidatorOptions(Action<IRuleValidator<TLeft, TRight>> setOptions)
        {
            if (_ruleValidator == null)
            {
                _ruleValidator = new RuleValidator<TLeft, TRight>();
            }

            setOptions.Invoke(_ruleValidator);

            IsValid = IsPresent ? 
                _isLeft ? IsLeftValid() : IsRightValid()
                  : false;
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
                    _ruleValidator.Dispose();
                }

                _disposed = true;
            }
        }

        public static Either<TLeft, TRight> Of(TLeft value) => new Either<TLeft, TRight>(value, _ruleValidator);
        public static Either<TLeft, TRight> Of(TRight value) => new Either<TLeft, TRight>(value, _ruleValidator);

        // private methods

        private void AssignScopeValues(RuleValidator<TLeft, TRight> validator, Type type, bool isLeft, bool isPresent, bool isValid)
        {
            _ruleValidator = validator;
            _currentType = type;
            _isLeft = isLeft;
            IsPresent = isPresent;
            IsValid = isValid;
        }

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(right, _ruleValidator);
        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left, _ruleValidator);

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();
    }

}
