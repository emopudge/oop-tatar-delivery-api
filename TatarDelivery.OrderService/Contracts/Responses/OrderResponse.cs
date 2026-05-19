namespace TatarDelivery.OrderService.Contracts.Responses;

public sealed record OrderResponse(
    int Id,
    int UserId,
    int AddressId,
    decimal TotalPrice,
    decimal DeliveryPrice,
    string Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyCollection<OrderItemResponse> Items,
    IReadOnlyCollection<OrderStatusHistoryResponse> StatusHistory
);