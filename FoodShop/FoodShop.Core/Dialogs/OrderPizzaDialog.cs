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
    public class OrderPizzaDialog : WaterfallDialog
    {
        private const string PIZZA_ORDER_STATE = "PizzaOrderState";
        private ConversationState _conversationState;

        private List<string> _allowedPizzaTypes;
        private List<string> _allowedPizzaSizes;
        private List<string> _addOrderToCartActions;

        public OrderPizzaDialog(ConversationState conversationState) : base(DialogNames.OrderPizza)
        {
            _conversationState = conversationState;
            AddStep(DeterminePizzaAzync);
            AddStep(DetermineSizeAsync);
            AddStep(ConfirmPizzaAsync);
            AddStep(AddPizzaToCartAsync);

            _allowedPizzaTypes = new List<string>
            {
                PizzaNames.BBQ_Delux,
                PizzaNames.Carbonara,
                PizzaNames.FourCheese,
                PizzaNames.ChickenRanch
            };

            _allowedPizzaSizes = new List<string>
            {
                Size.Small.ToString(),
                Size.Middle.ToString(),
                Size.Large.ToString()
            };

            _addOrderToCartActions = new List<string>
            {
                OrderAction.Yes,
                OrderAction.No
            };
        }

        public async Task<DialogTurnResult> DeterminePizzaAzync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var pizzas = conversationData.RecognitionContext.GetEntities(EntityTypes.Pizza);
            var pizzaSize = conversationData.RecognitionContext.GetEntities<Size>(EntityTypes.Size).FirstOrDefault();

            var pizzaTypes = pizzas?.Intersect(_allowedPizzaTypes);

            if (pizzaTypes == null || !pizzaTypes.Any())
            {
                stepContext.Values[PIZZA_ORDER_STATE] = new List<PizzaItem>();

                var message = "What kind of pizza do you want to order?";
                var retryMessage = "I didn't get it. Please choise again ";

                return await stepContext.PromptAsync(DialogNames.ChoisePromptDialog,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text(message),
                        RetryPrompt = MessageFactory.Text(retryMessage),
                        Choices = ChoiceFactory.ToChoices(_allowedPizzaTypes)
                    },
                    cancellationToken);
            }

            stepContext.Values[PIZZA_ORDER_STATE] = pizzaTypes.Select(p => new PizzaItem() { Name = p, Size = pizzaSize }).ToList();

            await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

            return await stepContext.ContinueDialogAsync();
        }

        public async Task<DialogTurnResult> DetermineSizeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var pizzaOrder = stepContext.Values[PIZZA_ORDER_STATE] as List<PizzaItem>;

            if (pizzaOrder == null || !pizzaOrder.Any())
            {
                var choice = stepContext.Result as FoundChoice;

                if (choice == null)
                    return await stepContext.ReplaceDialogAsync(DialogNames.UnsupportedDialog);

                var pizza = _allowedPizzaTypes.SingleOrDefault(p => p.Contains(choice.Value));

                if (pizza == null)
                {
                    return await stepContext.ReplaceDialogAsync(DialogNames.OrderPizza);
                }
                else
                {
                    pizzaOrder = new List<PizzaItem> { new PizzaItem() { Name = pizza } };
                    stepContext.Values[PIZZA_ORDER_STATE] = pizzaOrder;
                }
            }

            if (pizzaOrder.Any(p => p.Size == Size.None))
            {
                var message = "What size do you want?";
                var retryMessage = "I didn't get you. \r\n Please choise size from the list ";

                return await stepContext.PromptAsync(DialogNames.ChoisePromptDialog,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text(message),
                        RetryPrompt = MessageFactory.Text(retryMessage),
                        Choices = ChoiceFactory.ToChoices(_allowedPizzaSizes)
                    },
                    cancellationToken);
            }

            await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

            return await stepContext.ContinueDialogAsync();
        }

        public async Task<DialogTurnResult> ConfirmPizzaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());


            var pizzaOrder = stepContext.Values[PIZZA_ORDER_STATE] as List<PizzaItem>;
            if (pizzaOrder.Any(p => p.Size == Size.None))
            {
                var choice = stepContext.Result as FoundChoice;

                if (choice == null)
                    return await stepContext.ReplaceDialogAsync(DialogNames.UnsupportedDialog);


                var sizeName = _allowedPizzaSizes.SingleOrDefault(p => p.Contains(choice.Value));
                var pizzaSize = Size.None;

                Enum.TryParse(sizeName, out pizzaSize);

                if (pizzaSize == Size.None)
                {
                    return await stepContext.ReplaceDialogAsync(DialogNames.OrderPizza);
                }
                else
                {
                    pizzaOrder.ForEach(p => p.Size = pizzaSize);
                }
            }

            pizzaOrder.ForEach(p => p.Price = GetPrice(p));

            stepContext.Values[PIZZA_ORDER_STATE] = pizzaOrder;

            await stepContext.Context.SendActivityAsync(BuildConfirmOrderMessage(pizzaOrder));

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

        public async Task<DialogTurnResult> AddPizzaToCartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var choice = stepContext.Result as FoundChoice;

            if (choice == null)
                return await stepContext.ReplaceDialogAsync(DialogNames.UnsupportedDialog);

            var orderAction = _addOrderToCartActions.SingleOrDefault(p => p.Contains(choice.Value));

            if (orderAction == OrderAction.Yes)
            {
                var pizzaOrder = stepContext.Values[PIZZA_ORDER_STATE] as List<PizzaItem>;
                if (pizzaOrder != null)
                {
                    conversationData.Card.OrderItems.AddRange(pizzaOrder);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

                    var message = pizzaOrder.Count > 1 ? "Pizzas were added to your cart" : "Pizza was added to your cart";

                    await stepContext.Context.SendActivityAsync(message);

                    return await stepContext.ReplaceDialogAsync(DialogNames.ContinueOrder);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Pizza wasn't added to your cart.");
                return await stepContext.ReplaceDialogAsync(DialogNames.ContinueOrder);
            }

            return await stepContext.EndDialogAsync();
        }


        private string BuildConfirmOrderMessage(List<PizzaItem> pizzaOrder)
        {
            var message = new StringBuilder();
            pizzaOrder.ForEach(p => message.AppendLine($"Pizza - {p.Name}, Size - {p.Size}, Cost - {p.Price.ToString("C")}"));
            return message.ToString();
        }

        public double GetPrice(PizzaItem pizza)
        {
            switch (pizza.Name)
            {
                case PizzaNames.BBQ_Delux:
                    {
                        return pizza.Size == Size.Small ? 9 : pizza.Size == Size.Middle ? 12 : 15;
                        break;
                    }
                case PizzaNames.Carbonara:
                    {
                        return pizza.Size == Size.Small ? 6 : pizza.Size == Size.Middle ? 9 : 11;
                        break;
                    }
                case PizzaNames.ChickenRanch:
                    {
                        return pizza.Size == Size.Small ? 8.5 : pizza.Size == Size.Middle ? 10 : 12.7;
                        break;
                    }
                case PizzaNames.FourCheese:
                    {
                        return pizza.Size == Size.Small ? 12 : pizza.Size == Size.Middle ? 14 : 16;
                        break;
                    }
            }
            return 0;
        }
    }
}
