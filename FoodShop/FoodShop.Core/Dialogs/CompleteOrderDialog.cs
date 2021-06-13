using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class CompleteOrderDialog : Dialog
    {
        private ConversationState _conversationState;
        public CompleteOrderDialog(ConversationState conversationState) :base(DialogNames.CompleteOrder)
        {
            _conversationState = conversationState;
        }

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {

            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(dialogContext.Context, () => new ConversationData());

            var orderItems = conversationData.Card.OrderItems;

            if (orderItems != null || orderItems.Count > 0)
            {                
                var message = new StringBuilder();
                message.AppendLine("You order includes the next items");
                foreach (var item in orderItems)
                {
                    message.AppendLine(GetOrderItemDescription(item));
                }

                var totalCost = orderItems.Sum(i => i.Price);
                message.AppendLine($"Total cost: {totalCost.ToString("C")}");

                dialogContext.Context.SendActivityAsync(message.ToString());
            }
            else
            {
                dialogContext.Context.SendActivityAsync("Your cart is empty");
            }

            return await dialogContext.EndDialogAsync();
        }

        private string GetOrderItemDescription(IOrderItem orderItem)
        {
            if(orderItem is PizzaItem)
            {
                var pizza = (PizzaItem)orderItem;
                return $"Pizza - {pizza.Name}, Size - {pizza.Size}, Cost - {pizza.Price.ToString("C")}";
            }
            if (orderItem is BurgerOrderItem)
            {
                var pizza = (BurgerOrderItem)orderItem;
                return $"Burger - {pizza.Name}, Cost - {pizza.Price.ToString("C")}";
            }
            else
            {
                return $"{orderItem.Name}, Price - {orderItem.Price.ToString("C")}";
            }
        }
    }
}
