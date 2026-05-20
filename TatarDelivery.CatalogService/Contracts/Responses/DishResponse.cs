namespace TatarDelivery.CatalogService.Contracts.Responses;

public record DishResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    bool IsAvailable,
    int CategoryId);