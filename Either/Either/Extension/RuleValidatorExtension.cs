using Either.Model;
using Either.Rule;

namespace Either.Extension
{
    public static class RuleValidatorExtension
    {
        public static void AddLeftRules<TLeft, TRight>(this IRuleValidator<TLeft, TRight> validator, ValidationRule<TLeft>[] leftRules = null)
        {
            leftRules.ForEach(rule =>
            {
                validator.AddRule(rule.Name, rule.Validator);
            });
        }

        public static void AddRightRules<TLeft, TRight>(this IRuleValidator<TLeft, TRight> validator, ValidationRule<TRight>[] leftRules = null)
        {
            leftRules.ForEach(rule =>
            {
                validator.AddRule(rule.Name, rule.Validator);
            });
        }
    }
}
