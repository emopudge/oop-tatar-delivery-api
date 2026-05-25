namespace TatarDelivery.OrderService.Contracts.Responses;

public sealed record OrderStatusHistoryResponse(
    int Id,
    OrderStatus Status,
    DateTime ChangedAtUtc,
    string ChangedBy
);