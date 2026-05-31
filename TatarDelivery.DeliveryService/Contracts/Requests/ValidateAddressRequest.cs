using System.ComponentModel.DataAnnotations;

namespace TatarDelivery.DeliveryService.Contracts.Requests;

public sealed class ValidateAddressRequest
{
    [Range(-90, 90)] public double Lat { get; set; }
    [Range(-180, 180)] public double Lon { get; set; }
}