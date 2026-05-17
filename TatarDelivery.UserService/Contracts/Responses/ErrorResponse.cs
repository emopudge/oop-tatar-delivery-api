namespace TatarDelivery.UserService.Contracts.Responses;

public sealed record ErrorResponse(
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors = null);
