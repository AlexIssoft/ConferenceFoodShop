using FoodShop.CognitiveServices.Domain;
using System.Collections.Generic;

namespace FoodShop.Domain
{
    public class ConversationData
    {
        public string DialogId { get; set; }

        public RecognitionContext RecognitionContext { get; set; }

        private Cart _cart;
        public Cart Card
        {
            get => _cart ?? (_cart = new Cart() { OrderItems = new List<IOrderItem>() });            
        }
    }
}