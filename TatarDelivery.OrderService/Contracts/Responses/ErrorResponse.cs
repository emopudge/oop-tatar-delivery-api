namespace TatarDelivery.OrderService.Contracts.Responses;

public sealed record ErrorResponse(
    string Message,
    object? Errors = null
);