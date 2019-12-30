using Microsoft.VisualStudio.TestTools.UnitTesting;
using Either;

namespace Either.Test
{
    [TestClass]
    public class EitherTests
    {
        private Either<string, int> _either; 
        private RuleValidator<string, int> _validator;

        [Test]
        public void EitherConstructor_NoParametersGiven_InstantiatesRuleValidator()
        {
            _either = new Either<string, int>();
        }

        [Test]
        public void EitherConstructor_ValidatorProvided_InstantiatesRuleValidator()
        {
            _validator = new RuleValidator<string, int>(); 
            _either = new Either<string, int>(_validator);
        }

    }
}
