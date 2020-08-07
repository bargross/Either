using Either.Rule;
using Microsoft.Extensions.Logging;
using System;

namespace Either.Extension
{
    internal static class RuleValidatorLoggingExtension
    {
        public static void Log<TLeft, TRight>(
            this IRuleValidator<TLeft, TRight> validator, 
            ILogger<IRuleValidator<TLeft, TRight>> logger, 
            Model.LogLevel level, 
            Action action)
        {
            if(logger == null)
            {
                throw new MissingMemberException("No logger found");
            }

            try
            {
                action();
            }
            catch(Exception e)
            {
                switch (level) 
                {
                    case Model.LogLevel.Warning:
                        logger.LogWarning(e.Message, e);
                        break;
                    case Model.LogLevel.Debug:
                        logger.LogDebug(e.Message, e);
                        break;
                    case Model.LogLevel.Error:
                        logger.LogError(e.Message, e);
                        break;
                }
            }
        }
    }
}
