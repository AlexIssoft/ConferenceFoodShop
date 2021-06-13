using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.CognitiveServices.Domain;
using FoodShop.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace FoodShop.Core.Dialogs
{
    public class MainOrderDialog : WaterfallDialog
    {
        private ConversationState _conversationState;
        public MainOrderDialog(ConversationState conversationState) : base(DialogNames.Order)
        {
            _conversationState = conversationState;
            AddStep(StartOrderAsync);
            AddStep(ChoiceTypeAsync);
        }

        public async Task<DialogTurnResult> StartOrderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var pizzas = conversationData.RecognitionContext.GetEntities(EntityTypes.Pizza);
            if (pizzas != null && pizzas.Count > 0)
            {
                return await stepContext.ReplaceDialogAsync(DialogNames.OrderPizza);
            }

            var burgers = conversationData.RecognitionContext.GetEntities(EntityTypes.Burger);
            if (burgers != null && burgers.Count > 0)
            {
                return await stepContext.ReplaceDialogAsync(DialogNames.OrderBurger);
            }

            var drinks = conversationData.RecognitionContext.GetEntities(EntityTypes.Drinks);
            if (drinks != null && drinks.Count > 0)
            {
                return await stepContext.ReplaceDialogAsync(DialogNames.OrderDrink);
            }

            // There is no information in request about order

            var message = "What do you like to order?";
            var retryMessage = "I didn't get it. Please choise again ";
            var choises = new List<string>()
            {
                FoodNames.Pizza,
                FoodNames.Burger,
                FoodNames.Drink
            };

            return await stepContext.PromptAsync(DialogNames.ChoisePromptDialog, 
                new PromptOptions { 
                    Prompt = MessageFactory.Text(message), 
                    RetryPrompt = MessageFactory.Text(retryMessage), 
                    Choices = ChoiceFactory.ToChoices(choises) 
                },
                cancellationToken);
        }

        public async Task<DialogTurnResult> ChoiceTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = (FoundChoice)stepContext.Result;
            var dialogName = DialogNames.UnsupportedDialog;

            if(choice == null)
                return await stepContext.ReplaceDialogAsync(DialogNames.UnsupportedDialog);

            switch (choice.Value)
            {
                case FoodNames.Pizza:
                    dialogName = DialogNames.OrderPizza;
                    break;
                case FoodNames.Burger:
                    dialogName = DialogNames.OrderBurger;
                    break;
                case FoodNames.Drink:
                    dialogName = DialogNames.OrderDrink;
                    break;
                default:
                    break;
            }

            return await stepContext.ReplaceDialogAsync(dialogName);
        }

    }
}
