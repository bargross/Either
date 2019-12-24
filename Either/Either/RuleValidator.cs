using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Either
{
    public class RuleValidator<L, R> : IRuleValidator<L, R>
    {
        
        private IDictionary<string, Func<L, bool>> _rulesForLeft;
        private IDictionary<string, Func<R, bool>> _rulesForRight;
        private bool _initialized;

        public IList<string> FailedValidationMessages { get; private set; }
        public bool TerminateOnFail { get; set; }
        public bool IsLeftValue { get; set; }

        private void Init()
        {
            if(!_initialized)
            {
                _rulesForLeft = new Dictionary<string, Func<L, bool>>();
                _rulesForRight = new Dictionary<string, Func<R, bool>>();
                FailedValidationMessages = new List<string>(15) as IList<string>;

                _initialized = true;
            }

        }

        public void AddRule(Rule<L> rule)
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

            _rulesForLeft.Add(ruleName, ruleExpression.Compile());
        }

        public void AddRule(Rule<R> rule)
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

            _rulesForRight.Add(ruleName, ruleExpression.Compile());
        }

        public bool ValidateRuleFor(R value)
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

        public bool ValidateRuleFor(L value)
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

        public Rule<T> Pack<T>(string ruleName, Expression<Func<T, bool>> rule)
        {
            return new Rule<T>()
            {
                RuleName = ruleName,
                TypeRule = rule
            };
        }
    }
}