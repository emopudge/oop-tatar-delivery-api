namespace TatarDelivery.UserService.Domain;

public sealed class Address
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string City { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string House { get; set; } = string.Empty;

    public string? Apartment { get; set; }

    public string? Entrance { get; set; }

    public string? Comment { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
