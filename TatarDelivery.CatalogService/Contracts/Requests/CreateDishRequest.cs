using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.CatalogService.Contracts.Requests;

public record CreateDishRequest(
    [Required] string Name,
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть положительной")]
    decimal Price,
    [Range(1, int.MaxValue, ErrorMessage = "ID категории должен быть положительным")]
    int CategoryId,
    string? Description = null,
    bool IsAvailable = true
);