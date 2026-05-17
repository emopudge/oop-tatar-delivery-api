using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.UserService.Contracts.Requests;

public sealed class CreateAddressRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string City { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Street { get; init; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string House { get; init; } = string.Empty;

    [StringLength(10)]
    public string? Apartment { get; init; }

    [StringLength(5)]
    public string? Entrance { get; init; }

    [StringLength(255)]
    public string? Comment { get; init; }

    public bool IsDefault { get; init; }
}
