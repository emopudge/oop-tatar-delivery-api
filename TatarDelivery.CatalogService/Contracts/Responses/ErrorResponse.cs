namespace TatarDelivery.CatalogService.Contracts.Responses;

public record ErrorResponse(
    string Detail,
    string Code);