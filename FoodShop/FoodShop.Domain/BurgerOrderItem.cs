namespace FoodShop.Domain
{
    public class BurgerOrderItem : IOrderItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
