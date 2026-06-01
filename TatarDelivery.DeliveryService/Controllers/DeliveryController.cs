using Microsoft.AspNetCore.Mvc;
using TatarDelivery.DeliveryService.Contracts.Requests;
using TatarDelivery.DeliveryService.Contracts.Responses;
using TatarDelivery.DeliveryService.Services;

namespace TatarDelivery.DeliveryService.Controllers;

[ApiController]
[Route("delivery")]
public class DeliveryController : ControllerBase
{
    private readonly DeliveryValidationService _deliveryValidationService;

    public DeliveryController(DeliveryValidationService deliveryValidationService)
    {
        _deliveryValidationService = deliveryValidationService;
    }

    [HttpPost("validate-address")]
    public async Task<ActionResult<DeliveryValidationResponse>> Validate([FromBody] ValidateAddressRequest request)
    {
        var response = await _deliveryValidationService.ValidateAsync(request.Lat, request.Lon);

        return Ok(response);
    }
}
