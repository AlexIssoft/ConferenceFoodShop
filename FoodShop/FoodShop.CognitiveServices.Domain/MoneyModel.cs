using Newtonsoft.Json;

namespace FoodShop.CognitiveServices.Domain
{
    public class MoneyModel
    {
        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("number")]
        public decimal Number { get; set; }
    }
}
