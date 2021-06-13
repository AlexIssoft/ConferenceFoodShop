using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class UnsupportedDialog : Dialog
    {
        public UnsupportedDialog():base(DialogNames.UnsupportedDialog)
        {
        }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            dialogContext.Context.SendActivityAsync("Sorry! I don't understand your question.");
            dialogContext.Context.SendActivityAsync("Please rephrase your question");

            return dialogContext.EndDialogAsync();
        }
    }
}
