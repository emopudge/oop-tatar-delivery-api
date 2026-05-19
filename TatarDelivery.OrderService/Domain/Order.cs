namespace TatarDelivery.OrderService.Domain;

public sealed class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AddressId { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal DeliveryPrice { get; set; }

    public string Status { get; set; } = "PendingPayment";

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public List<OrderItem> Items { get; set; } = [];

    public List<OrderStatusHistory> StatusHistory { get; set; } = [];
}