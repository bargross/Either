using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Either.Extension;
using Either.Model;

namespace Either.Rule
{
    public class RuleValidator<TLeft, TRight> : IRuleValidator<TLeft, TRight>
    {

        /// <summary>
        /// 
        /// </summary>
        private IDictionary<string, (Func<TLeft, bool>, bool)> _rulesForLeft;
        private IDictionary<string, (Func<TRight, bool>, bool)> _rulesForRight;
        private readonly ILogger<IRuleValidator<TLeft, TRight>> _logger;


        /// <summary>
        /// 
        /// </summary>
        private bool _initialized;
        private bool _disposed = false;
        private int _capacity = 10;


        /// <summary>
        /// 
        /// </summary>
        public IList<string> FailedValidationMessages { get; private set; }
        public bool TerminateOnFail { get; set; }
        public bool IsLeftValue { get; set; }
        public int FailedCount { get; private set; }
        public int RuleCount { get; private set; }
        public bool LogException { get; set; }

        // TODO: add functionality for the other set of rules, so far only loglevel is being used.
        public RuleValidatorOptions Options { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public RuleValidator() => Init();
        public RuleValidator(ILogger<IRuleValidator<TLeft, TRight>> logger = null, RuleValidatorOptions options = null)
        {
            _logger = logger != null ? logger : null;

            if(options == null)
            {
                Options = GetDefaultOptions();
            } else
            {
                Options = options;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ~RuleValidator() => Dispose(false);


        /// <summary>
        /// 
        /// </summary>
        public IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TLeft, bool> rule)
        {
            if(LogException)
            {
                this.Log(_logger, Options.LogOptions.LogLevel, () => AddRule(ruleName, rule, _rulesForLeft));
            } 
            else
            {
                AddRule(ruleName, rule, _rulesForLeft);
            }

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        public IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TRight, bool> rule)
        {
            if (LogException)
            {
                this.Log(_logger, Options.LogOptions.LogLevel, () => AddRule(ruleName, rule, _rulesForRight));
            }
            else
            {
                AddRule(ruleName, rule, _rulesForRight);
            }

            AddRule(ruleName, rule, _rulesForRight);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TLeft, bool> replacement)
        {
            _rulesForLeft[ruleName] = (replacement, false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TRight, bool> replacement)
        {
            _rulesForRight[ruleName] = (replacement, false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ValidateRuleFor(TLeft value) => ValidateRuleFor(value, _rulesForLeft);
        public bool ValidateRuleFor(TRight value) => ValidateRuleFor(value, _rulesForRight);

        public void RemoveRulesForLeft() => _rulesForLeft.Clear();
        public void RemoveRulesForRight() => _rulesForRight.Clear();

        public void ResetRulesForLeft() => _rulesForLeft.SetValuesToDefault();
        public void ResetRulesForRight() => _rulesForRight.SetValuesToDefault();

        public bool ContainsRule(string ruleName) => _rulesForLeft.ContainsKey(ruleName) || _rulesForRight.ContainsKey(ruleName);

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void SetOptions(Action<RuleValidatorOptions> action)
        {
            if(Options == null)
            {
                var options = new RuleValidatorOptions();

                action(options);

                Options = options;
            } 
            else
            {
                action(Options);
            }
        }

        // Private Methods

        private RuleValidatorOptions GetDefaultOptions()
        {
            return new RuleValidatorOptions
            {
                ValidationOnAssigment = ValidationOnAssignment.Before,
                LogOptions = new LogOptions
                {
                    LogLevel = Model.LogLevel.Error,
                    ValidationError = ValidationError.FailThrow 
                }
            };
        }

        private void Init()
        {
            if (!_initialized)
            {
                _rulesForLeft = new Dictionary<string, (Func<TLeft, bool>, bool)>(_capacity);
                _rulesForRight = new Dictionary<string, (Func<TRight, bool>, bool)>(_capacity);
                FailedValidationMessages = new List<string>(_capacity);
                TerminateOnFail = false;

                _initialized = true;
            }
        }

        private void AddRule<T>(string ruleName, Func<T, bool> rule, IDictionary<string, (Func<T, bool>, bool)> ruleContainer) 
        {
            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule must have a name");
            }

            RuleCount++;

            ruleContainer.Add(ruleName, (rule, false) );
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

            for(int index = 0; index < ruleContainer.Count; ++index)
            {
                var ruleName = ruleContainer.Keys.ElementAt(index);
                var rule = ruleContainer[ruleName].Item1;

                if (TerminateOnFail && !rule.Invoke(value))
                {
                    FailedCount++;
                    
                    return false;
                }

                if (!TerminateOnFail && !rule.Invoke(value))
                {
                    FailedValidationMessages.Add($"Value failed rule {ruleName} on validation");

                    FailedCount++;
                    
                    continue;
                }

                ruleContainer[ruleName] = (ruleContainer[ruleName].Item1, true);
            }

            return !TerminateOnFail && (FailedValidationMessages.Count > 0 || true);
        }

        // GC

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