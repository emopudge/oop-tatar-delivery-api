namespace TatarDelivery.CatalogService.Contracts.Requests;

public record GetDishesRequest(
    int? CategoryId = null,
    string? Search = null);