namespace TatarDelivery.OrderService.Contracts.Responses;

public sealed record OrderStatusHistoryResponse(
    int Id,
    string Status,
    DateTime ChangedAtUtc,
    string ChangedBy
);