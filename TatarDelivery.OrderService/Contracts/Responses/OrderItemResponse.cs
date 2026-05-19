namespace TatarDelivery.OrderService.Contracts.Responses;

public sealed record OrderItemResponse(
    int Id,
    int DishId,
    int Quantity,
    decimal Price
);