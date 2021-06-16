using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class InitialGreetingDialog : Dialog
    {
        public InitialGreetingDialog():base(DialogNames.InitialGreetings)
        {
        }

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            await dialogContext .Context.SendActivityAsync("Hello! I am a virtual assistance. I can help you to order a food.");
            
            return await dialogContext.EndDialogAsync();
        }
    }
}
