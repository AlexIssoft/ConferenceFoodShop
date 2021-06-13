using System.Collections.Generic;

namespace FoodShop.CognitiveServices.Domain
{
    public class DateTimeResolution
    {
        public List<DateTimeResolutionEntity> Values { get; set; }

        public DateTimeResolution()
        {
            Values = new List<DateTimeResolutionEntity>();
        }
    }
}
