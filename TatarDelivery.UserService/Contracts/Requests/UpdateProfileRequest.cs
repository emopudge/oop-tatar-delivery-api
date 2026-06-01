using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.UserService.Contracts.Requests;

public sealed class UpdateProfileRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Телефон должен быть из 10-15 цифр и начинаться с +.")]
    public string Phone { get; init; } = string.Empty;
}
