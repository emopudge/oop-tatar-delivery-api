namespace TatarDelivery.UserService.Contracts.Responses;

public sealed record UserResponse(
    int Id,
    string Email,
    string FullName,
    string Phone,
    DateTime CreatedAtUtc);
