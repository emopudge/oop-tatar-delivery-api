// TatarDelivery.OrderService/Contracts/Responses/PaymentResponse.cs
using System.Text.Json.Serialization;

namespace TatarDelivery.OrderService.Contracts.Responses;

public class PaymentResponse  // 🔧 record → class
{
    [JsonPropertyName("paymentId")]
    public string? PaymentId { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}