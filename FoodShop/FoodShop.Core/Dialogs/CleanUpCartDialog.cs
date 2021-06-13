using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace FoodShop.Core.Dialogs
{
    public class CleanUpCartDialog : WaterfallDialog
    {
        private ConversationState _conversationState;
        public CleanUpCartDialog(ConversationState conversationState) : base(DialogNames.CleanUpCart)
        {
            _conversationState = conversationState;
            AddStep(AskConsumerAsync);
            AddStep(ChoiceTypeAsync);
        }

        public async Task<DialogTurnResult> AskConsumerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());

            var message = "Do you want to remove all your items from cart?";
            var retryMessage = "I didn't get it. Please choise again";
            var choises = new List<string>()
            {
                "Yes",
                "No"
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

            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(stepContext.Context, () => new ConversationData());


            var choice = (FoundChoice)stepContext.Result;

            switch (choice.Value)
            {
                case "Yes":
                    conversationData.Card.OrderItems.Clear();
                    break;
                case "No":
                    stepContext.Context.SendActivityAsync("Nothing is removed");
                    break;
                default:
                    break;
            }


            await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

            return await stepContext.EndDialogAsync();
        }

    }
}
