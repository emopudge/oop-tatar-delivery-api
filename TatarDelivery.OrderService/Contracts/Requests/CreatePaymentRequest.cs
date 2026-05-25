using System.Text.Json.Serialization;

namespace TatarDelivery.OrderService.Contracts.Requests;

public record CreatePaymentRequest(
    [property: JsonPropertyName("orderId")] string OrderId,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("description")] string Description);