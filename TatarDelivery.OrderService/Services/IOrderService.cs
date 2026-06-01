using TatarDelivery.OrderService.Domain;
using TatarDelivery.OrderService.Contracts.Responses;

namespace TatarDelivery.OrderService.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> FindOrderByIdAsync(int id);
    Task<IReadOnlyCollection<Order>> GetOrdersByUserIdAsync(int userId);
    Task<bool> TryApplyPaymentResultAsync(int orderId, PaymentResponse payment);
    Task<bool> TryMarkOrderAsPaidAsync(int orderId, string paymentId);
    Task<bool> TryCancelOrderAsync(int orderId);
    Task<bool> TryMarkOrderAsDeliveredAsync(int orderId);
}
