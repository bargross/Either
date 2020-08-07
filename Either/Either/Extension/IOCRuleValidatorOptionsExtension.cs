using Either.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Either.Extension
{
    public static class IOCRuleValidatorOptionsExtension
    {
        public static void SetValidatorOptions(this IServiceCollection services, Func<RuleValidatorOptions, RuleValidatorOptions> action)
        {
            var validatorOptions = action(new RuleValidatorOptions());

            var descriptor = ServiceDescriptor.Singleton(validatorOptions);
            services.Add(descriptor);
        }
    }
}
