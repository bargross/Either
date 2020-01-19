using System;
using System.Collections.Generic;

namespace Either.Rule
{
    public class RuleValidator<TLeft, TRight> : IRuleValidator<TLeft, TRight>, IDisposable
    {
        private IDictionary<string, (Func<TLeft, bool>, bool)> _rulesForLeft;
        private IDictionary<string, (Func<TRight, bool>, bool)> _rulesForRight;
        private bool _initialized;
        private bool _disposed = false;

        public IList<string> FailedValidationMessages { get; private set; }
        public bool TerminateOnFail { get; set; }
        public bool IsLeftValue { get; set; }
        public int FailedCount { get; private set; }
        public int RuleCount { get; private set; }

        public RuleValidator() => Init();
        ~RuleValidator() => Dispose(false);
        
        private void Init()
        {
            if(!_initialized)
            {
                _rulesForLeft = new Dictionary<string, (Func<TLeft, bool>, bool)>();
                _rulesForRight = new Dictionary<string, (Func<TRight, bool>, bool)>();
                FailedValidationMessages = new List<string>(15) as IList<string>;
                TerminateOnFail = false;

                _initialized = true;
            }
        }

        public void AddRule(Rule<TLeft> rule) => AddRule(rule, _rulesForLeft);
        public void AddRule(Rule<TRight> rule) => AddRule(rule, _rulesForRight);

        public void Replace(string ruleName, Func<TLeft, bool> replacement) => _rulesForLeft[ruleName] = (replacement, false);
        public void Replace(string ruleName, Func<TRight, bool> replacement) => _rulesForRight[ruleName] = (replacement, false);

        public bool ValidateRuleFor(TRight value) => ValidateRuleFor(value, _rulesForRight);
        public bool ValidateRuleFor(TLeft value) => ValidateRuleFor(value, _rulesForLeft);

        public void ResetRulesForLeft() => _rulesForLeft.Clear();
        public void ResetRulesForRight() => _rulesForRight.Clear();

        public bool GetRuleValidationResult(string ruleName)
        {
            if(_rulesForLeft.ContainsKey(ruleName))
            {
                return _rulesForLeft[ruleName].Item2;
            }

            if(_rulesForRight.ContainsKey(ruleName))
            {
                return _rulesForRight[ruleName].Item2;
            }

            throw new KeyNotFoundException("Rule not found");
        }

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

        // Private Methods

        private void AddRule<T>(Rule<T> rule, IDictionary<string, (Func<T, bool>, bool)> ruleContainer) 
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

            RuleCount++;

            ruleContainer.Add(ruleName, (ruleExpression, false) );
        }

        private bool ValidateRuleFor<T>(T value, IDictionary<string, (Func<T, bool>, bool)> ruleContainer)
        {
            if(value == null)
            {
                throw new ArgumentException("Value is null");
            }

            if(ruleContainer.Count == 0)
            {
                return true;
            }

            foreach(var ruleName in ruleContainer.Keys)
            {
                var rule = ruleContainer[ruleName].Item1;
                if (!TerminateOnFail && !rule(value))
                {
                    FailedValidationMessages.Add($"Value failed rule {ruleName} on validation");
                    FailedCount++;
                    continue;
                }

                if(TerminateOnFail && !rule(value))
                {
                    FailedCount++;
                    return false;
                }

                ruleContainer[ruleName] = (ruleContainer[ruleName].Item1, true);
            }

            if(!TerminateOnFail && FailedValidationMessages.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}