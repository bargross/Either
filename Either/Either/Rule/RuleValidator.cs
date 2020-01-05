using System;
using System.Collections.Generic;

namespace Either.Rule
{
    public class RuleValidator<TLeft, TRight> : IRuleValidator<TLeft, TRight>, IDisposable
    {
        
        private IDictionary<string, Func<TLeft, bool>> _rulesForLeft;
        private IDictionary<string, Func<TRight, bool>> _rulesForRight;
        private bool _initialized;
        private bool _disposed = false;

        public IList<string> FailedValidationMessages { get; private set; }
        public bool TerminateOnFail { get; set; }
        public bool IsLeftValue { get; set; }

        public RuleValidator() => Init();
        ~RuleValidator() => Dispose(false);
        
        private void Init()
        {
            if(!_initialized)
            {
                _rulesForLeft = new Dictionary<string, Func<TLeft, bool>>();
                _rulesForRight = new Dictionary<string, Func<TRight, bool>>();
                FailedValidationMessages = new List<string>(15) as IList<string>;

                _initialized = true;
            }

        }

        public void AddRule(Rule<TLeft> rule)
        {
            var ruleName = rule.RuleName;
            var ruleExpression = rule.TypeRule;

            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule must have a name");
            }

            if(rule == null)
            {
                throw new NullReferenceException("Rule cannot be null");
            }

            _rulesForLeft.Add(ruleName, ruleExpression);
        }

        public void AddRule(Rule<TRight> rule)
        {
            var ruleName = rule.RuleName;
            var ruleExpression = rule.TypeRule;

            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule must have a name");
            }

            if(rule == null)
            {
                throw new NullReferenceException("Rule cannot be null");
            }

            _rulesForRight.Add(ruleName, ruleExpression);
        }

        public void Replace(string ruleName, Func<TLeft, bool> replacement) => _rulesForLeft[ruleName] = replacement;
        public void Replace(string ruleName, Func<TRight, bool> replacement) => _rulesForRight[ruleName] = replacement;

        public bool ValidateRuleFor(TRight value)
        {
            if(value == null)
            {
                throw new ArgumentException("Value is null");
            }

            if(_rulesForRight.Count == 0)
            {
                return true;
            }

            foreach(var ruleName in _rulesForRight.Keys)
            {
                var rule = _rulesForRight[ruleName];
                
                if(!TerminateOnFail && !rule(value))
                {
                    FailedValidationMessages.Add($"Rule {ruleName} failed validation");
                    continue;
                }

                if(!rule(value))
                {
                    return false;
                }
            }

            if(!TerminateOnFail && FailedValidationMessages.Count > 0)
            {
                return false;
            }

            return true;
        }

        public bool ValidateRuleFor(TLeft value)
        {
            if(value == null)
            {
                throw new ArgumentException("Value is null");
            }

            if(_rulesForRight.Count == 0)
            {
                return true;
            }

            foreach(var ruleName in _rulesForLeft.Keys)
            {
                var rule = _rulesForLeft[ruleName];
                
                if(!TerminateOnFail && !rule(value))
                {
                    FailedValidationMessages.Add($"Rule {ruleName} failed validation");
                    continue;
                }

                if(!rule(value))
                {
                    return false;
                }
            }

            if(!TerminateOnFail && FailedValidationMessages.Count > 0)
            {
                return false;
            }

            return true;
        }

        public void ResetRulesForLeft() => _rulesForLeft.Clear();
        public void ResetRulesForRight() => _rulesForRight.Clear();

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
                    // managed resources

                    _rulesForLeft = null;
                    _rulesForRight = null;
                    FailedValidationMessages = null;
                }

                _disposed = true;
            }
        }
    }
}