using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Either;
using Either.Rule;
using Either.Exceptions;

namespace EitherTests
{
    [TestClass]
    public class EitherTests
    {
        private Either<string, int> _either;
        private RuleValidator<string, int> _validator;

        [TestInitialize]
        public void Setup()
        {
            _either = new Either<string, int>();
            _validator = new RuleValidator<string, int>();
        }

        [TestMethod]
        public void AddRule_RuleNameNotProvidedForLeft_ThrowsArgumentExcption()
        {
            Assert.ThrowsException<ArgumentException>(() => _either.AddRule(null, value => !string.IsNullOrWhiteSpace(value)));
        }

        public void AddRule_RuleNameNotProvidedForRight_ThrowsArgumentExcption()
        {
            Assert.ThrowsException<ArgumentException>(() => _either.AddRule(null, value => value > 0 && value < 10));
        }

        [TestMethod]
        public void AddRule_RuleProvidedForLeft_ReturnsValueBetweenBounds()
        {
            var expected = "bla";

            _either.AddRule("B", value => !string.IsNullOrWhiteSpace(value));

            _either = expected;

            Assert.AreEqual(expected, _either.GetValue<string>());
        }

        [TestMethod]
        public void AddRule_RuleProvidedForRight_ReturnsValueBetweenBounds()
        {
            var expected = 1;

            _either.AddRule("B", value => value > 0 && value < 10);

            _either = expected;

            Assert.AreEqual(expected, _either.GetValue<int>());
        }

        [TestMethod]
        public void AddRule_RuleProvidedForLeftWithValueOutsideOfBounds_ThrowsRuleValidationException()
        {
            var expected = " ";

            _either.AddRule("A", value => !string.IsNullOrWhiteSpace(value));

            _either = expected;

            Assert.ThrowsException<RuleValidationException>(() => _either.GetValue<string>());
        }

        [TestMethod]
        public void AddRule_RuleProvidedForRightWithValueOutsideOfBounds_ThrowsRuleValidationException()
        {
            var expected = 11;

            _either.AddRule("A", value => value > 0 && value < 10);

            _either = expected;

            Assert.ThrowsException<RuleValidationException>(() => _either.GetValue<int>());
        }
    }
}
