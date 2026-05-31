using System.Text.Json.Serialization;

namespace TatarDelivery.OrderService.Contracts.Responses;

public class PaymentResponse
{
    [JsonPropertyName("paymentId")]
    public string? PaymentId { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}