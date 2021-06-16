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

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            await dialogContext.Context.SendActivityAsync("Hello! I can help you to order a food \r\n What do you like to order?");

            return await dialogContext.EndDialogAsync();
        }
    }
}
