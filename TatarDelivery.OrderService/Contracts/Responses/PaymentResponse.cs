using System.Text.Json.Serialization;

namespace TatarDelivery.OrderService.Contracts.Responses;

public record PaymentResponse(
    [property: JsonPropertyName("paymentId")] string PaymentId,
    [property: JsonPropertyName("status")] string Status);