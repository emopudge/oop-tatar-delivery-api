namespace TatarDelivery.CatalogService.Contracts.Responses;

public record DishResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable,
    int CategoryId);