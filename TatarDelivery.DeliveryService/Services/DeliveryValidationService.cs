using TatarDelivery.DeliveryService.Contracts.Responses;

namespace TatarDelivery.DeliveryService.Services;

public class DeliveryValidationService
{
    private readonly IGeocodingService _geo;
    
    // Моковая база ресторанов (ID, Широта, Долгота, Радиус км)
    private static readonly (int Id, double Lat, double Lon, double Radius)[] _restaurants = 
    [ 
        (1, 55.790, 49.110, 5.0), 
        (2, 55.785, 49.120, 3.0), 
        (3, 55.800, 49.100, 7.0) 
    ];

    public DeliveryValidationService(IGeocodingService geo) => _geo = geo;

    public async Task<DeliveryValidationResponse> ValidateAsync(double lat, double lon)
    {
        var addr = await _geo.GetAddressAsync(lat, lon);
        
        // Ищем ближайший ресторан, в радиус которого попадает точка
        var nearest = _restaurants
            .Select(r => (r.Id, Dist: Haversine(lat, lon, r.Lat, r.Lon), r.Radius))
            .Where(x => x.Dist <= x.Radius)
            .OrderBy(x => x.Dist)
            .FirstOrDefault();

        // Если не нашли ни одного ресторана в радиусе
        if (nearest.Id == 0) return new(false, addr, null, null);

        // Расчет времени: 20 мин готовка + 2 мин на каждый км
        return new(true, addr, nearest.Id, 20 + (int)Math.Ceiling(nearest.Dist * 2));
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Радиус Земли
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;
        
        // ✅ ИСПРАВЛЕНО: вместо **2 используем умножение
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + 
                   Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * 
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                   
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}