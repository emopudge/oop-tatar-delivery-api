namespace TatarDelivery.UserService.Contracts.Responses;

public sealed record AddressResponse(
    int Id,
    string City,
    string Street,
    string House,
    string? Apartment,
    string? Entrance,
    string? Comment,
    bool IsDefault);
