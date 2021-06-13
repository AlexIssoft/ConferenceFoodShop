using Newtonsoft.Json;
using System.Collections.Generic;

namespace FoodShop.CognitiveServices.Domain
{
    public class DateTimeModel
    {
        [JsonProperty("values")]
        public List<DateTimeValueModel> Values { get; set; }

        public DateTimeModel()
        {
            Values = new List<DateTimeValueModel>();
        }
    }
}
