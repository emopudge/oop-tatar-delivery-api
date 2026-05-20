using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.OrderService.Contracts.Requests;

public sealed class CreateOrderItemRequest
{
    [Range(1, int.MaxValue)]
    public int DishId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}