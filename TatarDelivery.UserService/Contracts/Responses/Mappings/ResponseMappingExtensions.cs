using TatarDelivery.UserService.Domain;

namespace TatarDelivery.UserService.Contracts.Responses.Mappings;

public static class ResponseMappingExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(
            user.Id,
            user.Email,
            user.FullName,
            user.Phone,
            user.CreatedAtUtc);

    public static AddressResponse ToResponse(this Address address) =>
        new(
            address.Id,
            address.City,
            address.Street,
            address.House,
            address.Apartment,
            address.Entrance,
            address.Comment,
            address.IsDefault);
}
