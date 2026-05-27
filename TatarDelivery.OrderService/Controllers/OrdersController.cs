using Microsoft.AspNetCore.Mvc;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using TatarDelivery.OrderService.Services;
using System.Threading.Tasks;
using TatarDelivery.OrderService.Contracts.Responses.Mappings;

namespace TatarDelivery.OrderService.Controllers;

[ApiController]
[Route("orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
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
        order.DeliveryPrice = Math.Round(itemsTotal * 0.1m, 2);
        order.TotalPrice = itemsTotal + order.DeliveryPrice;

        order.StatusHistory.Add(new Domain.OrderStatusHistory
        {
            Status = OrderStatus.PendingPayment,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = "user"
        });

        var createdOrder = await _orderService.CreateOrderAsync(order);

        return StatusCode(StatusCodes.Status201Created, createdOrder.ToResponse());
    }

    private static decimal GetMockDishPrice(int dishId) => dishId switch
    {
        1 => 350m,
        2 => 420m,
        3 => 280m,
        4 => 500m,
        5 => 250m,
        _ => 300m
    };
}