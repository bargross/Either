using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Either.Rule;

namespace EitherTests
{
    [TestClass]
    public class RuleValidatorTests
    {
        private RuleValidator<int, string> _ruleValidator;

        [TestInitialize]
        public void Setup()
        {
            _ruleValidator = new RuleValidator<int, string>();
        }

        [TestMethod]
        public void AddRule_RuleNameForLeftIsNull_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _ruleValidator.AddRule(null, value => value > 0);
            });
        }

        [TestMethod]
        public void AddRule_RuleNameForRightIsNull_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _ruleValidator.AddRule(null, value => !string.IsNullOrEmpty(value));
            });
        }

        [TestMethod]
        public void AddRule_RuleProvidedForLeft_RuleExists()
        {
            var ruleName = "greater than 0";
            _ruleValidator.AddRule(ruleName, value => value > 0);

            Assert.IsTrue(_ruleValidator.RuleCount == 1);
            Assert.IsTrue(_ruleValidator.ContainsRule(ruleName));
        }

        [TestMethod]
        public void AddRule_RuleProvidedForRight_RuleExists()
        {
            var ruleName = "not null or empty";
            _ruleValidator.AddRule(ruleName, value => !string.IsNullOrEmpty(value));

            Assert.IsTrue(_ruleValidator.RuleCount == 1);
            Assert.IsTrue(_ruleValidator.ContainsRule(ruleName));
        }

        [TestMethod]
        public void AddRule_RuleProvidedForLeftAndRight_RulesExists()
        {
            var ruleNameForRight = "not null or empty";
            var ruleNameForLeft = "greater than 0";

            _ruleValidator
                .AddRule(ruleNameForLeft, value => !string.IsNullOrEmpty(value))
                .AddRule(ruleNameForRight, value => value > 0);

            Assert.IsTrue(_ruleValidator.RuleCount == 2);
            Assert.IsTrue(_ruleValidator.ContainsRule(ruleNameForLeft));
            Assert.IsTrue(_ruleValidator.ContainsRule(ruleNameForRight));
        }
    }
}
