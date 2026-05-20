using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.OrderService.Contracts.Requests;

public sealed class CreateOrderRequest
{
    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = [];

    [Range(1, int.MaxValue)]
    public int AddressId { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}