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
            order.PaymentId,
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

    public static OrderItemResponse MapToOrderResponse(this OrderItem orderItem)
    {
        if (orderItem is null)
        {
            throw new ArgumentNullException(nameof(orderItem));
        }

        return new OrderItemResponse(
            Id: orderItem.Id,
            DishId: orderItem.DishId,
            Quantity: orderItem.Quantity,
            Price: orderItem.Price
        );
    }

    public static OrderStatusHistoryResponse MapToOrderStatusHistoryResponse(this OrderStatusHistory statusHistory)
    {
        if (statusHistory is null)
        {
            throw new ArgumentNullException(nameof(statusHistory));
        }

        return new OrderStatusHistoryResponse(
            Id: statusHistory.Id,
            Status: statusHistory.Status,
            ChangedAtUtc: statusHistory.ChangedAtUtc,
            ChangedBy: statusHistory.ChangedBy
        );
    }
}