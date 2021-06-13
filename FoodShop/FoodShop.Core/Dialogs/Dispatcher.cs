using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FoodShop.CognitiveServices.Core;
using FoodShop.CognitiveServices.Domain;
using FoodShop.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace FoodShop.Core.Dialogs
{
    public class Dispatcher : Dialog
    {
        private BotState _conversationState { get; set; }
        private readonly INaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;
        public Dispatcher(ConversationState conversationState, INaturalLanguageUnderstandingService naturalLanguageUnderstandingService): base(DialogNames.Dispatcher)
        {
            _conversationState = conversationState;
            _naturalLanguageUnderstandingService = naturalLanguageUnderstandingService;
        }
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, object options = null, CancellationToken cancellationToken = default)
        {
            if (dialogContext.Context.Activity.Type == ActivityType.Message)
            {
                var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
                var conversationData = await conversationContext.GetAsync(dialogContext.Context, () => new ConversationData());

                var recognitionResult = await _naturalLanguageUnderstandingService.RecognizeAsync(new CognitiveServices.Domain.RecognitionRequest()
                {
                    ActivityText = dialogContext.Context.Activity.Text
                });

                conversationData.RecognitionContext = recognitionResult;
                await conversationContext.SetAsync(dialogContext.Context, conversationData);
                await _conversationState.SaveChangesAsync(dialogContext.Context, false, cancellationToken);

                return await ResolveDialogAsync(dialogContext);

            }

            return await dialogContext.EndDialogAsync();
        }

        public async Task<DialogTurnResult> ResolveDialogAsync(DialogContext dialogContext)
        {
            var conversationContext = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationContext.GetAsync(dialogContext.Context, () => new ConversationData());

            var intent = conversationData.RecognitionContext.TopScoringIntent;

            var dialogName = GetDialogName(intent);            

            if (string.IsNullOrEmpty(dialogName))
            {
                return await dialogContext.EndDialogAsync();
            }

            await dialogContext.CancelAllDialogsAsync();
            return await dialogContext.ReplaceDialogAsync(dialogName);
        }

        private string GetDialogName(IntentProperty intent)
        {
            if(intent.Score < 0.4)
            {
                return DialogNames.UnsupportedDialog;
            }

            switch (intent.Name)
            {
                case IntentNames.Greetings:
                    {
                        return DialogNames.Greetings;
                        break;
                    }
                case IntentNames.Order:
                    {
                        return DialogNames.Order;
                        break;
                    }
                case IntentNames.Delivery:
                    {
                        return DialogNames.Delivery;
                        break;
                    }
                case IntentNames.CompleteOrder:
                    {
                        return DialogNames.CompleteOrder;
                        break;
                    }
                case IntentNames.CleanUpCart:
                    {
                        return DialogNames.CleanUpCart;
                        break;
                    }
                default:
                    {
                        return DialogNames.UnsupportedDialog;
                    }
            }
        }
    }
}
