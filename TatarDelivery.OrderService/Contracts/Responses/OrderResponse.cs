namespace TatarDelivery.OrderService.Contracts.Responses;

public sealed record OrderResponse(
    int Id,
    int UserId,
    int AddressId,
    decimal TotalPrice,
    decimal DeliveryPrice,
    OrderStatus Status,
    string? PaymentID,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyCollection<OrderItemResponse> Items,
    IReadOnlyCollection<OrderStatusHistoryResponse> StatusHistory
);