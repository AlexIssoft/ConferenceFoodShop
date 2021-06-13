using System.Collections.Generic;

namespace FoodShop.CognitiveServices.Domain
{
    public class ListResolution
    {
        public List<string> Values { get; set; }

        public ListResolution()
        {
            Values = new List<string>();
        }
    }
}
