using TatarDelivery.OrderService.Domain;

namespace TatarDelivery.OrderService.Contracts.Responses.Mappings;

public static class OrderMappings
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse(
            order.Id,
            order.UserId,
            order.AddressId,
            order.TotalPrice,
            order.DeliveryPrice,
            order.Status,
            order.CreatedAtUtc,
            order.UpdatedAtUtc,
            order.Items.Select(item => new OrderItemResponse(
                item.Id,
                item.DishId,
                item.Quantity,
                item.Price
            )).ToList(),
            order.StatusHistory.Select(history => new OrderStatusHistoryResponse(
                history.Id,
                history.Status,
                history.ChangedAtUtc,
                history.ChangedBy
            )).ToList()
        );
    }
}