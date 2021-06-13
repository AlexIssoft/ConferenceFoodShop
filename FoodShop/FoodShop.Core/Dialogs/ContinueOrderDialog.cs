using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class ContinueOrderDialog : Dialog
    {
        public ContinueOrderDialog() :base(DialogNames.ContinueOrder)
        {
        }

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            dialogContext.Context.SendActivityAsync("Would you like to order anything else?");

            return await dialogContext.EndDialogAsync();
        }                
    }
}
