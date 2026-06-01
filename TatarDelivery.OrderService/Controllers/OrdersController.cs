using Microsoft.AspNetCore.Mvc;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using TatarDelivery.OrderService.Contracts.Responses.Mappings;
using TatarDelivery.OrderService.Services;
using System.Threading.Tasks;
using TatarDelivery.OrderService.Domain;

namespace TatarDelivery.OrderService.Controllers;

[ApiController]
[Route("orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private const decimal DeliveryPriceRate = 0.1m;
    private const decimal DefaultMockDishPrice = 300m;

    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest(new ErrorResponse("В заказе должно быть хотя бы что-то."));
        }

        var order = new Domain.Order
        {
            UserId = request.UserId,
            AddressId = request.AddressId,
            RestaurantId = request.RestaurantId,
            Status = OrderStatus.PendingPayment,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        foreach (var item in request.Items)
        {
            var price = GetMockDishPrice(item.DishId);
            order.Items.Add(new Domain.OrderItem
            {
                DishId = item.DishId,
                Quantity = item.Quantity,
                Price = price
            });
        }

        var itemsTotal = order.Items.Sum(i => i.Price * i.Quantity);
        order.DeliveryPrice = Math.Round(itemsTotal * DeliveryPriceRate, 2);
        order.TotalPrice = itemsTotal + order.DeliveryPrice;

        order.StatusHistory.Add(new Domain.OrderStatusHistory
        {
            Status = OrderStatus.PendingPayment,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = "user"
        });

        var createdOrder = await _orderService.CreateOrderAsync(order);
        var orderResponse = OrderMappings.MapToOrderResponse(createdOrder);
        return StatusCode(StatusCodes.Status201Created, orderResponse);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await _orderService.FindOrderByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        var orderResponse = OrderMappings.MapToOrderResponse(order);
        return Ok(orderResponse);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetMyOrders([FromQuery] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new ErrorResponse("Некорректный идентификатор пользователя."));
        }

        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        var response = orders
            .Select(OrderMappings.MapToOrderResponse)
            .ToList();

        return Ok(response);
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var success = await _orderService.TryCancelOrderAsync(id);

        if (!success)
        {
            var existingOrder = await _orderService.FindOrderByIdAsync(id);
            if (existingOrder is null)
            {
                return NotFound(new ErrorResponse("Заказ не найден."));
            }
            return BadRequest(new ErrorResponse("Заказ не может быть отменён в нынешнем статусе."));
        }

        var updatedOrder = await _orderService.FindOrderByIdAsync(id);
        if (updatedOrder is null)
        {
            return NotFound(new ErrorResponse("Заказ не найден после попытки отмены."));
        }

        var orderResponse = updatedOrder.ToResponse();
        return Ok(orderResponse);
    }

    [HttpPost("{id:int}/deliver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkOrderAsDelivered(int id)
    {
        var success = await _orderService.TryMarkOrderAsDeliveredAsync(id);

        if (!success)
        {
            var existingOrder = await _orderService.FindOrderByIdAsync(id);
            if (existingOrder is null)
            {
                return NotFound(new ErrorResponse("Заказ не найден."));
            }

            return BadRequest(new ErrorResponse("Заказ не может быть доставлен в нынешнем статусе."));
        }

        var updatedOrder = await _orderService.FindOrderByIdAsync(id);
        if (updatedOrder is null)
        {
            return NotFound(new ErrorResponse("Заказ не найден после попытки смены статуса доставки."));
        }

        var orderResponse = OrderMappings.MapToOrderResponse(updatedOrder);
        return Ok(orderResponse);
    }

    [HttpPost("{id:int}/pay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PayOrder(int id)
    {
        var order = await _orderService.FindOrderByIdAsync(id);
        if (order is null)
        {
            return NotFound(new ErrorResponse("Заказ не найден."));
        }

        if (string.IsNullOrEmpty(order.PaymentId))
        {
            return BadRequest(new ErrorResponse("Нет идентификатора оплаты для этого заказа."));
        }

        var success = await _orderService.TryMarkOrderAsPaidAsync(id, order.PaymentId);
        if (success)
        {
            return Ok(new { message = "Статус оплаты обновлён. Заказ оплачен.", orderStatus = order.Status });
        }

        return BadRequest(new { message = "Не удалось подтвердить оплату. Проверьте статус платежа.", orderStatus = order.Status });
    }

    private static decimal GetMockDishPrice(int dishId) => dishId switch
    {
        1 => 350m,
        2 => 420m,
        3 => 280m,
        4 => 500m,
        5 => 250m,
        _ => DefaultMockDishPrice
    };
}
