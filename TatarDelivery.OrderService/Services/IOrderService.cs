using TatarDelivery.OrderService.Domain;

namespace TatarDelivery.OrderService.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> FindOrderByIdAsync(int id);
    Task<bool> TryMarkOrderAsPaidAsync(int orderId, string paymentId);
}