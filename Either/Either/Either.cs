using System;
using Either.Root;
using Either.Rule;
using Either.Exceptions;
using System.Linq.Expressions;

namespace Either
{
    public class Either<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private RootEither<TLeft, TRight> _root;
        private static IRuleValidator<TLeft, TRight> _ruleValidator;

        private Type _currentType;
        private bool _isLeft;
        private bool _disposed = false;

        public bool IsValid { get; private set; }
        public bool IsPresent {
            get
            {
                return _root != null && (_root.IsLeftPresent || _root.IsRightPresent);
            }
        }

        public Either() => _ruleValidator = new RuleValidator<TLeft, TRight>();

        private Either(TLeft left, IRuleValidator<TLeft, TRight> validator)
        {
            _root = left;

            var ruleValidator = _ruleValidator == null ? new RuleValidator<TLeft, TRight>() : validator;
            var isLeftValid = IsLeftValid();

            AssignScopeValues(ruleValidator, typeof(TLeft), true, isLeftValid);
        }

        private Either(TRight right, IRuleValidator<TLeft, TRight> validator)
        {
            _root = right;

            var ruleValidator = _ruleValidator == null ? new RuleValidator<TLeft, TRight>() : validator;
            var isRightValid = IsRightValid();

            AssignScopeValues(ruleValidator, typeof(TRight), false, isRightValid);
        }

        ~Either() => Dispose(false);

        /// <summary>
        /// Replaces a pre-existing rule for the left value
        /// throws ArgumentException if rule does not exist
        /// </summary>
        /// <param name="ruleName"></param>
        /// <param name="replacement"></param>
        public void ReplaceRule(string ruleName, Expression<Func<TLeft, bool>> replacement)
        {
            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }
            
            _ruleValidator.Replace(ruleName, replacement.Compile());
        }

        /// <summary>
        /// Replaces a pre-existing rule for the right value
        /// throws ArgumentException if rule does not exist
        /// </summary>
        /// <param name="ruleName"></param>
        /// <param name="replacement"></param>
        public void ReplaceRule(string ruleName, Expression<Func<TRight, bool>> replacement)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule name cannot be null or empty");
            }
            
            _ruleValidator.Replace(ruleName, replacement.Compile());
        }

        public bool IsLeftValid() => _ruleValidator.ValidateRuleFor(_root.Left);
        public bool IsRightValid() => _ruleValidator.ValidateRuleFor(_root.Right);

        /// <summary>
        /// Gets the value T where T must be either type left or right
        /// throws InvalidCastException if type does not match either
        /// throws RuleValidationException if type does not meet specified rules
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns type="T"></returns>
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

            throw new InvalidCastException($"Either {typeof(TLeft)} nor {typeof(TRight)} match type: {nameof(type)}");
        }
        
        /// <summary>
        /// Removes all pre-existing rules for the left value
        /// </summary>
        public void RemoveLeftRules() => _ruleValidator.RemoveRulesForLeft();

        /// <summary>
        /// Removes all pre-existing rules for the right value
        /// </summary>
        public void RemoveRightRules() => _ruleValidator.RemoveRulesForRight();

        /// <summary>
        /// Resets all pre-existing rules results for the left value
        /// </summary>
        public void ResetLeftRulesToDefault() => _ruleValidator.ResetRulesForLeft();

        /// <summary>
        /// Resets all pre-existing rules results for the right value
        /// </summary>
        public void ResetRightRulesToDefault() => _ruleValidator.ResetRulesForRight();

        /// <summary>
        /// Resets validation rules, 
        /// </summary>
        public void RemoveAllRules()
        {
            RemoveLeftRules();
            RemoveRightRules();
        }

        /// <summary>
        /// Resets all rules for both the left and right assignments
        /// </summary>
        public void ResetAllResultsToDefault()
        {
            ResetLeftRulesToDefault();
            ResetRightRulesToDefault();
        }

        /// <summary>
        /// Gets the validation result for a specific rule by rule name
        /// </summary>
        /// <param name="ruleName"></param>
        /// <returns type="bool"></returns>
        public bool GetValidationResultFor(string ruleName) => _ruleValidator.GetRuleValidationResult(ruleName);
        public bool ContainsRule(string ruleName) => _ruleValidator.ContainsRule(ruleName);

        public static Either<TLeft, TRight> Of(TLeft value) => new Either<TLeft, TRight>(value, _ruleValidator);
        public static Either<TLeft, TRight> Of(TRight value) => new Either<TLeft, TRight>(value, _ruleValidator);


        // private methods

        private void AssignScopeValues(IRuleValidator<TLeft, TRight> validator, Type type, bool isLeft, bool isValid)
        {
            _ruleValidator = validator;
            _currentType = type;
            _isLeft = isLeft;
            IsValid = isValid;
        }

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left, _ruleValidator);
        public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(right, _ruleValidator);

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();


        /// <summary>
        /// IDisposeable implementation, disposes of all managed resources in type Either
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _ruleValidator.Dispose();
                }

                _disposed = true;
            }
        }
    }

}
