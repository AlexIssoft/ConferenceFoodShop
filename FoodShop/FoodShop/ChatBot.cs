using FoodShop.Domain;
using FoodShop.Core.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace FoodShopBot
{
    public class ChatBot : IBot
    {
        private const string EVENT = "event";
        private const string MESSAGE = "message";
        private const string CONVERSATIN_UPDATE = "conversationUpdate";

        private ConversationState _conversationState;
        private INaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;

        private readonly DialogSet _dialogSet;
        private ComponentDialog _container;

        public ChatBot(ConversationState conversationState, INaturalLanguageUnderstandingService naturalLanguageUnderstandingService)
        {
            _conversationState = conversationState;
            _naturalLanguageUnderstandingService = naturalLanguageUnderstandingService;
            var dialogState = conversationState.CreateProperty<DialogState>(nameof(DialogState));
            _dialogSet = new DialogSet(dialogState);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            switch (turnContext.Activity.Type)
            {
                case EVENT:
                case MESSAGE:
                    {
                        initDialogs();

                        var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                        await dialogContext.ContinueDialogAsync(cancellationToken);

                        if (!turnContext.Responded)
                        {
                            await dialogContext.BeginDialogAsync(DialogNames.Dispatcher, null, cancellationToken);
                        }
                        break;
                    }


                case CONVERSATIN_UPDATE:
                    {
                        initDialogs();

                        var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                        if (dialogContext.Context.Activity.MembersAdded != null)
                        {
                            foreach (var member in dialogContext.Context.Activity.MembersAdded)
                            {
                                if (member.Id != dialogContext.Context.Activity.Recipient.Id)
                                {
                                    await dialogContext.ReplaceDialogAsync(DialogNames.InitialGreetings);
                                }
                            }
                        }
                        break;
                    }
            }
            _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private async void initDialogs()
        {
            _dialogSet.Add(new Dispatcher(_conversationState, _naturalLanguageUnderstandingService));

            _dialogSet.Add(new InitialGreetingDialog());
            _dialogSet.Add(new GreetingDialog());

            _dialogSet.Add(new MainOrderDialog(_conversationState));
            _dialogSet.Add(new OrderPizzaDialog(_conversationState));
            _dialogSet.Add(new OrderDrinkDialog());
            _dialogSet.Add(new OrderBurgerDialog(_conversationState));

            _dialogSet.Add(new ChoisePromptDialog());
            _dialogSet.Add(new ContinueOrderDialog());
            _dialogSet.Add(new CompleteOrderDialog(_conversationState));
            _dialogSet.Add(new UnsupportedDialog());
            _dialogSet.Add(new CleanUpCartDialog(_conversationState));
        }
    }
}
