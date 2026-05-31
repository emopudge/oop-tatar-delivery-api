using Microsoft.AspNetCore.Mvc;
using TatarDelivery.DeliveryService.Contracts.Requests;
using TatarDelivery.DeliveryService.Contracts.Responses;
using TatarDelivery.DeliveryService.Services;

namespace TatarDelivery.DeliveryService.Controllers;

[ApiController]
[Route("delivery")]
public class DeliveryController : ControllerBase
{
    private readonly DeliveryValidationService _svc;
    public DeliveryController(DeliveryValidationService svc) => _svc = svc;

    [HttpPost("validate-address")]
    public async Task<ActionResult<DeliveryValidationResponse>> Validate([FromBody] ValidateAddressRequest req)
        => Ok(await _svc.ValidateAsync(req.Lat, req.Lon));
}