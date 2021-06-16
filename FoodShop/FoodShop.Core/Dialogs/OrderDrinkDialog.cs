using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class OrderDrinkDialog : Dialog
    {
        public OrderDrinkDialog():base(DialogNames.OrderDrink)
        {
        }

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            await dialogContext.Context.SendActivityAsync("Sorry! You cannot order drinks now.");

            return await dialogContext.ReplaceDialogAsync(DialogNames.ContinueOrder);
        }
    }
}
