using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class GreetingDialog : Dialog
    {
        public GreetingDialog():base(DialogNames.Greetings)
        {
        }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            dialogContext.Context.SendActivityAsync("Hello! I can help you to order a food");
            dialogContext.Context.SendActivityAsync("What do you like to order?");

            return dialogContext.EndDialogAsync();
        }
    }
}
