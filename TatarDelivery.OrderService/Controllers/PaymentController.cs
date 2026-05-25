using Microsoft.AspNetCore.Mvc;
using TatarDelivery.OrderService.Clients;
using TatarDelivery.OrderService.Contracts.Responses;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentClient _paymentClient;

    public PaymentController(IPaymentClient paymentClient)
    {
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
    }

    [HttpGet("{paymentId}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentStatus(string paymentId)
    {
        if (string.IsNullOrWhiteSpace(paymentId))
            return BadRequest("Необходим PaymentID.");

        try
        {
            var response = await _paymentClient.GetPaymentStatusAsync(paymentId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ошибка сервера", details = ex.Message });
        }
    }
}