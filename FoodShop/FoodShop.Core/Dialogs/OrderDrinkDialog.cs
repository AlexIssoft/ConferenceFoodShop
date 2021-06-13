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

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            dialogContext.Context.SendActivityAsync("What do you order to drink?");

            return dialogContext.EndDialogAsync();
        }
    }
}
