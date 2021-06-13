using System;
using System.Collections.Generic;
using System.Text;

namespace FoodShop.Domain
{
    public interface IOrderItem
    {
        string Name { get; set; }

        double Price { get; set; }
    }
}
