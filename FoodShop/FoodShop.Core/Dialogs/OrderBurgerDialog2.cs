using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class OrderBurgerDialog2 : Dialog
    {
        public OrderBurgerDialog2():base(DialogNames.OrderBurger)
        {
        }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            dialogContext.Context.SendActivityAsync("What a burger do you want to order?");

            return dialogContext.EndDialogAsync();
        }
    }
}
