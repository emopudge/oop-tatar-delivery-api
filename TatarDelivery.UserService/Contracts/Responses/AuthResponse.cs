namespace TatarDelivery.UserService.Contracts.Responses;

public sealed record AuthResponse(
    string TokenType,
    string AccessToken,
    UserResponse User);
