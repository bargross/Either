using Either.Model;
using Either.Rule;
using Microsoft.Extensions.DependencyInjection;

namespace Either.Extension
{
    public static class IOCRuleValidatorExtension
    {
        public static void AddRuleValidator<TLeft, TRight>(
            this IServiceCollection services, 
            ValidationRule<TLeft>[] leftRules = null,
            ValidationRule<TRight>[] rightRules = null)
        {
            if(leftRules == null && rightRules == null)
            {
                services.AddSingleton<IRuleValidator<TLeft, TRight>, RuleValidator<TLeft, TRight>>();

                return;
            }

            var validator = new RuleValidator<TLeft, TRight>();
            if(leftRules != null)
            {
                validator.AddLeftRules(leftRules);
            }

            if(rightRules != null)
            {
                validator.AddRightRules(rightRules);
            }

            var descriptor = ServiceDescriptor.Singleton(validator);
            services.Add(descriptor);
        }
    }
}
