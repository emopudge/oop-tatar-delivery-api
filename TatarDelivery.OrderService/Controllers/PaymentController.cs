using Microsoft.AspNetCore.Mvc;
using TatarDelivery.OrderService.Clients;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using TatarDelivery.OrderService.Services;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentClient _paymentClient;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentClient paymentClient,
        IOrderService orderService,
        ILogger<PaymentController> logger)
    {
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        if (request is null)
            return BadRequest(new ErrorResponse("Запрос не может быть null."));

        try
        {
            var response = await _paymentClient.CreatePaymentAsync(request);

            if (int.TryParse(request.OrderId, out var orderId))
            {
                await _orderService.TryApplyPaymentResultAsync(orderId, response);
            }

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP ошибка при создании платежа.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("Ошибка внешнего сервиса."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при создании платежа.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("Внутренняя ошибка сервера."));
        }
    }

    [HttpGet("{paymentId}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentStatus(string paymentId)
    {
        if (string.IsNullOrWhiteSpace(paymentId))
            return BadRequest(new ErrorResponse("PaymentId обязателен."));

        try
        {
            var response = await _paymentClient.GetPaymentStatusAsync(paymentId);
            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP ошибка при получении статуса платежа {PaymentId}.", paymentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("Ошибка внешнего сервиса."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при получении статуса платежа {PaymentId}.", paymentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("Внутренняя ошибка сервера."));
        }
    }
}
