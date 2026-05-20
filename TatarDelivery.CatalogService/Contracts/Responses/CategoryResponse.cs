namespace TatarDelivery.CatalogService.Contracts.Responses;

public record CategoryResponse(
    int Id,
    string Name,
    string Description);