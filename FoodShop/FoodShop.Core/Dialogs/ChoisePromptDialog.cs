using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace FoodShop.Core.Dialogs
{
    public class ChoisePromptDialog: ChoicePrompt
    {
        public ChoisePromptDialog() : base(DialogNames.ChoisePromptDialog, AttemptPromptValidatorAsync)
        {            
        }
        private static Task<bool> AttemptPromptValidatorAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            var foundMatches = promptContext.Options.Choices.Select(c => c.Value).Contains(promptContext.Context.Activity.Text, StringComparer.InvariantCultureIgnoreCase);
            if(foundMatches)
                return Task.FromResult(true);

            if (promptContext.AttemptCount >= 2)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
