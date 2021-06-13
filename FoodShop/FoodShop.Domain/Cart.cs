using System;
using System.Collections.Generic;
using System.Text;

namespace FoodShop.Domain
{
    public class Cart
    {
        public List<IOrderItem> OrderItems { get; set; }
    }
}
