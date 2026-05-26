namespace TatarDelivery.DeliveryService.Contracts.Responses;

public sealed record DeliveryValidationResponse(
    bool IsDeliverable,
    string? Address,
    int? RestaurantId,
    int? EstimatedMinutes);