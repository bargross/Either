using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Either;
using Either.Exceptions;

namespace EitherTests
{
    [TestClass]
    public class EitherTests
    {
        private Either<string, int> _either;

        [TestInitialize]
        public void Setup()
        {
            _either = new Either<string, int>();
        }

        [TestMethod]
        public void SetValidationRules_RuleNameNotProvidedForLeft_ThrowsArgumentExcption()
        {
            Assert.ThrowsException<ArgumentException>(() => {
                _either.SetValidationRules(options => options.AddRule(null, value => !string.IsNullOrWhiteSpace(value)) );
            });
        }

        [TestMethod]
        public void SetValidationRules_RuleNameNotProvidedForRight_ThrowsArgumentExcption()
        {
            Assert.ThrowsException<ArgumentException>(() => {
                _either.SetValidationRules(options => options.AddRule(null, value => value > 0 && value < 10));
            });
        }

        [TestMethod]
        public void ContainsRule_RuleNameNotProvided_ReturnsFalse() => Assert.IsFalse(_either.ContainsRule("A"));

        [TestMethod]
        public void ContainsRule_RuleNameProvided_ReturnsTrue()
        {
            var ruleName = "A";
            _either.SetValidationRules( options => options.AddRule(ruleName, value => value == ruleName));

            var result = _either.ContainsRule(ruleName);

            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void SetValidationRules_RuleProvidedForLeft_ValidatorContainsRule()
        {
            var ruleName = "B";

            _either.SetValidationRules( options => {
                options.AddRule(ruleName, value => !string.IsNullOrWhiteSpace(value));
            });

            Assert.IsTrue(_either.ContainsRule(ruleName));
        }

        [TestMethod]
        public void SetValidationRules_RuleProvidedForRight_ValidatorContainsRule()
        {
            var ruleName = "B";

            _either.SetValidationRules(options => {
                options.AddRule(ruleName, value => value > 0 && value < 10);
            });

            Assert.IsTrue(_either.ContainsRule(ruleName));
        }

        [TestMethod]
        public void GetValue_RuleProvidedForLeftWithValueOutsideOfBounds_ThrowsInvalidCastException()
        {
            _either = " ";

            Assert.ThrowsException<InvalidCastException>(() => _either.GetValue<int>());
        }

        [TestMethod]
        public void GetValue_RuleProvidedForRightWithValueOutsideOfBounds_ThrowsInvalidCastException()
        {
            _either = 11;

            Assert.ThrowsException<InvalidCastException>(() => _either.GetValue<string>());
        }

        [TestMethod]
        public void GetValue_RuleProvidedForLeftWithValueOutsideOfBounds_ThrowsRuleValidationException()
        {
            var invalidValue = " ";
            var ruleName = "A";

            _either.SetValidationRules(options => {
                options.TerminateOnFail = true;
                options.AddRule(ruleName, value => !string.IsNullOrWhiteSpace(value));
            });

            _either = invalidValue;

            Assert.ThrowsException<RuleValidationException>(() => _either.GetValue<string>());
            Assert.IsFalse(_either.GetValidationResultForRule(ruleName));
            Assert.IsFalse(_either.IsValid);
        }

        [TestMethod]
        public void GetValue_RuleProvidedForRightWithValueOutsideOfBounds_ThrowsRuleValidationException()
        {
            var invalidValue = 11;
            var ruleName = "A";

            _either.SetValidationRules(options => {
                options.TerminateOnFail = true;
                options.AddRule(ruleName, value => value >= 0 && value <= 10);
            });

            _either = invalidValue;

            Assert.ThrowsException<RuleValidationException>(() => _either.GetValue<int>());
            Assert.IsFalse(_either.GetValidationResultForRule(ruleName));
            Assert.IsFalse(_either.IsValid);
        }

        [TestMethod]
        public void GetValue_RuleProvidedForLeftWithValueBetweenOfBounds_ReturnsLeftValue()
        {
            var expected = "Valid String";
            var ruleName = "A";

            _either.SetValidationRules(options => {
                options.TerminateOnFail = true;
                options.AddRule(ruleName, value => !string.IsNullOrWhiteSpace(value));
            });

            _either = expected;

            Assert.IsTrue(_either.GetValidationResultForRule(ruleName));
            Assert.IsTrue(_either.IsValid);
            Assert.IsTrue(_either.ContainsRule(ruleName));
            Assert.AreEqual(expected, _either.GetValue<string>());
        }

        [TestMethod]
        public void GetValue_RuleProvidedForRightWithValueBetweenOfBounds_ReturnsRightValue()
        {
            var expected = 10;
            var ruleName = "A";

            _either.SetValidationRules(options => {
                options.TerminateOnFail = true;
                options.AddRule(ruleName, value => value >= 0 && value <= 10);
            });

            _either = expected;

            Assert.IsTrue(_either.GetValidationResultForRule(ruleName));
            Assert.IsTrue(_either.IsValid);
            Assert.IsTrue(_either.ContainsRule(ruleName));
            Assert.AreEqual(expected, _either.GetValue<int>());
        }

        [TestMethod]
        public void ImplicitOperatorTLeft_ValidLeftValueProvided_CastsEitherToTLeft()
        {
            var expected = "bla";
            _either = expected;

            var result = (string)_either;

            Assert.IsTrue(_either.IsPresent);
            Assert.IsTrue(_either.IsValid);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ImplicitOperatorTRight_ValidRightValueProvided_CastsEitherToTRight()
        {
            var expected = 123123;
            _either = expected;

            var result = (int)_either;

            Assert.IsTrue(_either.IsPresent);
            Assert.IsTrue(_either.IsValid);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReplaceRule_NewRuleGivenToReplaceOldRule_ReplacesRuleForRight()
        {
            var ruleName = "C";

            _either.SetValidationRules(options => options.AddRule(ruleName, value => value == ruleName));

            _either.ReplaceRule(ruleName, value => value == 1);
            _either = 1;

            Assert.IsTrue(_either.ContainsRule(ruleName));
            Assert.IsTrue(_either.IsValid);
        }

        [TestMethod]
        public void ReplaceRule_NewRuleGivenToReplaceOldRule_ReplacesRuleForLeft()
        {
            var ruleName = "C";

            _either.SetValidationRules(options => options.AddRule(ruleName, value => value == 1));

            _either.ReplaceRule(ruleName, value => value == ruleName);
            _either = ruleName;

            Assert.IsTrue(_either.ContainsRule(ruleName));
            Assert.IsTrue(_either.IsValid);
        }

        [TestMethod]
        public void ResetRules_RulesProvidedForLeftAndRight_RulesAreCleared()
        {
            var ruleNameLeft = "A";
            var ruleNameRight = "B";

            _either.SetValidationRules(options => {
                options.AddRule(ruleNameLeft, value => value == ruleNameRight)
                .AddRule(ruleNameRight, value => value > 1);
            });

            var leftRuleBeforeClearing = _either.ContainsRule(ruleNameLeft);
            var rightRuleBeforeClearing = _either.ContainsRule(ruleNameRight);

            _either.ResetRules();

            var resultForLeft = _either.ContainsRule(ruleNameLeft);
            var resultForRight = _either.ContainsRule(ruleNameRight);

            Assert.IsTrue(leftRuleBeforeClearing);
            Assert.IsTrue(rightRuleBeforeClearing);
            Assert.IsFalse(resultForLeft);
            Assert.IsFalse(resultForRight);
        }
    }
}
