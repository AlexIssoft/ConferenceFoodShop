using System;
using System.Collections.Generic;
using System.Text;

namespace FoodShop.Domain
{
    public class PizzaItem : IOrderItem
    {
        public string Name { get; set; }
        public double Price { get; set; }

        public Size Size { get; set; }
    }
}
