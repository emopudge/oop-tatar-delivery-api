using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using TatarDelivery.OrderService.Contracts.Responses.Mappings;
using TatarDelivery.OrderService.Data;
using TatarDelivery.OrderService.Domain;

namespace TatarDelivery.OrderService.Controllers;

[ApiController]
[Route("orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public OrdersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest(new ErrorResponse("Order must contain at least one item."));
        }

        var now = DateTime.UtcNow;

        var order = new Order
        {
            UserId = request.UserId,
            AddressId = request.AddressId,
            Status = "PendingPayment",
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        foreach (var requestItem in request.Items)
        {
            var itemPrice = GetMockDishPrice(requestItem.DishId);

            order.Items.Add(new OrderItem
            {
                DishId = requestItem.DishId,
                Quantity = requestItem.Quantity,
                Price = itemPrice
            });
        }

        var itemsTotal = order.Items.Sum(item => item.Price * item.Quantity);
        order.DeliveryPrice = Math.Round(itemsTotal * 0.1m, 2);
        order.TotalPrice = itemsTotal + order.DeliveryPrice;

        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = order.Status,
            ChangedAtUtc = now,
            ChangedBy = "user"
        });

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, order.ToResponse());
    }
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await _dbContext.Orders
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound(new ErrorResponse("Order not found."));
        }

        return Ok(order.ToResponse());
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetMyOrders(
    [FromQuery] int userId)
    {
        var orders = await _dbContext.Orders
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .AsNoTracking()
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync();

        return Ok(orders.Select(order => order.ToResponse()).ToList());
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> CancelOrder(int id)
    {
        var order = await _dbContext.Orders
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound(new ErrorResponse("Order not found."));
        }

        if (order.Status != "PendingPayment")
        {
            return BadRequest(new ErrorResponse("Order cannot be cancelled in current status."));
        }

        var now = DateTime.UtcNow;

        order.Status = "Cancelled";
        order.UpdatedAtUtc = now;
        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = order.Status,
            ChangedAtUtc = now,
            ChangedBy = "user"
        });

        await _dbContext.SaveChangesAsync();

        return Ok(order.ToResponse());
    }

    [HttpPost("{id:int}/pay")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> PayOrder(int id)
    {
        var order = await _dbContext.Orders
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound(new ErrorResponse("Order not found."));
        }

        if (order.Status != "PendingPayment")
        {
            return BadRequest(new ErrorResponse("Only orders with PendingPayment status can be paid."));
        }

        var now = DateTime.UtcNow;

        order.Status = "Paid";
        order.UpdatedAtUtc = now;
        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = order.Status,
            ChangedAtUtc = now,
            ChangedBy = "user"
        });

        await _dbContext.SaveChangesAsync();

        return Ok(order.ToResponse());
    }

    [HttpPost("{id:int}/deliver")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> DeliverOrder(int id)
    {
        var order = await _dbContext.Orders
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound(new ErrorResponse("Order not found."));
        }

        if (order.Status != "Paid")
        {
            return BadRequest(new ErrorResponse("Only paid orders can be delivered."));
        }

        var now = DateTime.UtcNow;

        order.Status = "Delivered";
        order.UpdatedAtUtc = now;
        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = order.Status,
            ChangedAtUtc = now,
            ChangedBy = "delivery"
        });

        await _dbContext.SaveChangesAsync();

        return Ok(order.ToResponse());
    }

    private static decimal GetMockDishPrice(int dishId)
    {
        return dishId switch
        {
            1 => 350m,
            2 => 420m,
            3 => 280m,
            4 => 500m,
            5 => 250m,
            _ => 300m
        };
    }
}
