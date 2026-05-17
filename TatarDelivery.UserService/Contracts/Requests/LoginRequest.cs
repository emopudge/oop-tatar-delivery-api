using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.UserService.Contracts.Requests;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
