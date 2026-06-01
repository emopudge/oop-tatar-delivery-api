namespace TatarDelivery.DeliveryService.Services;

public class YandexGeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;
    public YandexGeocodingService(HttpClient http, IConfiguration cfg) { _http = http; _cfg = cfg; }

    public async Task<string?> GetAddressAsync(double lat, double lon)
    {
        await Task.Delay(10);  // Mock
        return $"Казань, ул. Тестовая, {Math.Abs((int)lon)}";
    }
}