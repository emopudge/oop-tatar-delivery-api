namespace TatarDelivery.DeliveryService.Services;

public interface IGeocodingService
{
    Task<string?> GetAddressAsync(double lat, double lon);
}