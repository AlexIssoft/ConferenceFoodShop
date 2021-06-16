using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.CognitiveServices.Domain;
using FoodShop.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace FoodShop.Core.Dialogs
{
    public class OrderBurgerDialog : WaterfallDialog
    {
        private const string BURGER_ORDER_STATE = "BurgerOrderState";
        private ConversationState _conversationState;

        private List<string> _allowedBurgerTypes;
        private List<string> _addOrderToCartActions;

        public OrderBurgerDialog(ConversationState conversationState) : base(DialogNames.OrderBurger)
        {
            _conversationState = conversationState;
            AddStep(DetermineBurgerAzync);
            AddStep(ConfirmBurgerAsync);
            AddStep(AddBurgerToCartAsync);

            _allowedBurgerTypes = new List<string>
            {
                BurgerNames.BigTasty,
                BurgerNames.Cheeseburger,
                BurgerNames.Chickenburger,
                BurgerNames.McChicken
            };

            _addOrderToCartActions = new List<string>
            {
                OrderAction.Yes,
                OrderAction.No
            };
        }

        public async Task<DialogTurnResult> DetermineBurgerAzync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var burger = conversationData.RecognitionContext.GetEntities(EntityTypes.Burger);

            var burgerTypes = burger?.Intersect(_allowedBurgerTypes);

            if (burgerTypes == null || !burgerTypes.Any())
            {
                stepContext.Values[BURGER_ORDER_STATE] = new List<BurgerOrderItem>();

                var message = "Which burger do you want to order?";
                var retryMessage = "I didn't get it. Please choise again ";

                return await stepContext.PromptAsync(DialogNames.ChoisePromptDialog,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text(message),
                        RetryPrompt = MessageFactory.Text(retryMessage),
                        Choices = ChoiceFactory.ToChoices(_allowedBurgerTypes)
                    },
                    cancellationToken);
            }

            stepContext.Values[BURGER_ORDER_STATE] = burgerTypes.Select(p => new BurgerOrderItem() { Name = p }).ToList();

            await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

            return await stepContext.ContinueDialogAsync();
        }

        public async Task<DialogTurnResult> ConfirmBurgerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var burgerOrder = stepContext.Values[BURGER_ORDER_STATE] as List<BurgerOrderItem>;

            if (burgerOrder == null || !burgerOrder.Any())
            {
                var choice = stepContext.Result as FoundChoice;

                if (choice == null)
                    return await stepContext.ReplaceDialogAsync(DialogNames.UnsupportedDialog);

                var burger = _allowedBurgerTypes.SingleOrDefault(p => p.Contains(choice.Value));

                if (burger == null)
                {
                    return await stepContext.ReplaceDialogAsync(DialogNames.OrderBurger);
                }
                else
                {
                    burgerOrder = new List<BurgerOrderItem> { new BurgerOrderItem() { Name = burger } };
                    stepContext.Values[BURGER_ORDER_STATE] = burgerOrder;
                }
            }

            burgerOrder.ForEach(p => p.Price = GetPrice(p));

            stepContext.Values[BURGER_ORDER_STATE] = burgerOrder;

            await stepContext.Context.SendActivityAsync(BuildConfirmOrderMessage(burgerOrder));

            var message = "Do you want to add that to your cart?";
            var retryMessage = "I didn't get you. Should I add your order to your cart?";

            return await stepContext.PromptAsync(DialogNames.ChoisePromptDialog,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(message),
                    RetryPrompt = MessageFactory.Text(retryMessage),
                    Choices = ChoiceFactory.ToChoices(_addOrderToCartActions)
                },
                cancellationToken);
        }

        public async Task<DialogTurnResult> AddBurgerToCartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var choice = stepContext.Result as FoundChoice;

            if (choice == null)
                return await stepContext.ReplaceDialogAsync(DialogNames.UnsupportedDialog);

            var orderAction = _addOrderToCartActions.SingleOrDefault(p => p.Contains(choice.Value));

            if (orderAction == OrderAction.Yes)
            {
                var burgerOrder = stepContext.Values[BURGER_ORDER_STATE] as List<BurgerOrderItem>;
                if (burgerOrder != null)
                {
                    conversationData.Card.OrderItems.AddRange(burgerOrder);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

                    var message = burgerOrder.Count > 1 ? "Burgers were added to your cart" : "Burger was added to your cart";

                    await stepContext.Context.SendActivityAsync(message);

                    return await stepContext.ReplaceDialogAsync(DialogNames.ContinueOrder);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Burger wasn't added to your cart. /n/r/ Do you want to order anything else?");
                return await stepContext.ReplaceDialogAsync(DialogNames.ContinueOrder);
            }

            return await stepContext.EndDialogAsync();
        }


        private string BuildConfirmOrderMessage(List<BurgerOrderItem> pizzaOrder)
        {
            var message = new StringBuilder();
            pizzaOrder.ForEach(p => message.AppendLine($"Burger - {p.Name}, Cost - {p.Price.ToString("C")}"));
            return message.ToString();
        }

        public double GetPrice(BurgerOrderItem burger)
        {
            switch (burger.Name)
            {
                case BurgerNames.BigTasty:
                    {
                        return 5.5;
                        break;
                    }
                case BurgerNames.Cheeseburger:
                    {
                        return 4;
                        break;
                    }
                case BurgerNames.Chickenburger:
                    {
                        return 4.5;
                        break;
                    }
                case BurgerNames.McChicken:
                    {
                        return 6;
                        break;
                    }
            }
            return 0;
        }
    }
}
