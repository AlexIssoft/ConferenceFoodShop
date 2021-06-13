using Newtonsoft.Json;
using System;

namespace FoodShop.CognitiveServices.Domain
{
    public class DateTimeResolutionModel
    {
        [JsonProperty("value")]
        public DateTime Value { get; set;}

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime? End { get; set; }

    }
}
