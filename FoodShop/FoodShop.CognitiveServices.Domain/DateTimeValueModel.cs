using Newtonsoft.Json;
using System.Collections.Generic;

namespace FoodShop.CognitiveServices.Domain
{
    public class DateTimeValueModel
    {
        [JsonProperty("timex")]
        public string Timex { get; set; }

        [JsonProperty("resolution")]
        public List<DateTimeResolutionModel> Resolution { get; set; }
    }
}
