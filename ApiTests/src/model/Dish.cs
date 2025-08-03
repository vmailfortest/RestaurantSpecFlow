namespace ApiTests.model;

public class Dish
{
    public DishType DishType { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderedAt { get; set; }
}