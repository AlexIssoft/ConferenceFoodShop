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

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            await dialogContext .Context.SendActivityAsync("Sorry! I don't understand your question.\r\n Please rephrase your question");

            return await dialogContext.EndDialogAsync();
        }
    }
}
