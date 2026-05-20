namespace TatarDelivery.OrderService.Domain;

public sealed class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int DishId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public Order Order { get; set; } = null!;
}