namespace TatarDelivery.OrderService.Domain;

public sealed class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime ChangedAtUtc { get; set; }

    public string ChangedBy { get; set; } = string.Empty;

    public Order Order { get; set; } = null!;
}