using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.UserService.Contracts.Requests;

public sealed class UpdateProfileRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone must contain 10 to 15 digits and may start with +.")]
    public string Phone { get; init; } = string.Empty;
}
