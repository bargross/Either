using NUnit.Framework;

using Either;
using Either.Rule;

namespace EitherTest
{
    public class EitherTests
    {
        private Either<string, int> _either;
        private RuleValidator<string, int> _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new RuleValidator<string, int>();
        }

        [Test]
        public void EitherConstructor_NoParametersProvided_ValidatorInstantiated()
        {
            _either = new Either<string, int>();

            Assert.IsTrue(_either.ValidatorInstantiated);
        }

        [Test]
        public void EitherConstructor_NoParametersProvided_ValidatorInstantiated()
        {
            _either = new Either<string, int>();

            Assert.IsTrue(_either.ValidatorInstantiated);
        }
    }
}