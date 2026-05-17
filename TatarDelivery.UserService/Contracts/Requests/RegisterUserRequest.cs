using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.UserService.Contracts.Requests;

public sealed class RegisterUserRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone must contain 10 to 15 digits and may start with +.")]
    public string Phone { get; init; } = string.Empty;
}
