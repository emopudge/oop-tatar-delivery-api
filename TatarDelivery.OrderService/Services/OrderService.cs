using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using TatarDelivery.OrderService.Clients;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using TatarDelivery.OrderService.Data;
using TatarDelivery.OrderService.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Services;

public class OrderService : IOrderService
{
    private const string ConfirmedPaymentStatus = "CONFIRMED";
    private const int PaymentRetryCount = 3;
    private static readonly TimeSpan PaymentTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan BaseRetryDelay = TimeSpan.FromMilliseconds(500);

    private readonly AppDbContext _context;
    private readonly IPaymentClient _paymentClient;
    private readonly ILogger<OrderService> _logger;
    private readonly IAsyncPolicy<PaymentResponse> _retryPolicy;
    private readonly IAsyncPolicy<PaymentResponse> _timeoutPolicy;
    private readonly IAsyncPolicy<PaymentResponse> _combinedPolicy;


    public OrderService(
        AppDbContext context,
        IPaymentClient paymentClient,
        ILogger<OrderService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _retryPolicy = Policy<PaymentResponse>
            .HandleResult(response => response.Status != ConfirmedPaymentStatus)
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: PaymentRetryCount,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * BaseRetryDelay.TotalMilliseconds));

        _timeoutPolicy = Policy.TimeoutAsync<PaymentResponse>(PaymentTimeout);
        _combinedPolicy = _retryPolicy.WrapAsync(_timeoutPolicy);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));
        if (order.Items.Count == 0)
            throw new ArgumentException("В заказе должно быть хотя бы что-то.", nameof(order));

        order.Status = OrderStatus.PendingPayment;
        order.CreatedAtUtc = DateTime.UtcNow;
        order.UpdatedAtUtc = DateTime.UtcNow;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Заказ {OrderId} создан. Статус: PendingPayment", order.Id);

        try
        {
            var paymentRequest = new CreatePaymentRequest(
                order.Id.ToString(),
                order.TotalPrice,
                $"Оплата заказа {order.Id}"
            );

            var result = await _combinedPolicy.ExecuteAsync(async (CancellationToken ct) =>
            {
                _logger.LogInformation("Пробуем создать оплату для заказа {OrderId}...", order.Id);
                return await _paymentClient.CreatePaymentAsync(paymentRequest);
            }, CancellationToken.None);

            if (result.Status == ConfirmedPaymentStatus)
            {
                var changedAtUtc = DateTime.UtcNow;

                order.Status = OrderStatus.Paid;
                order.PaymentId = result.PaymentId;
                order.UpdatedAtUtc = changedAtUtc;
                order.StatusHistory.Add(CreateStatusHistory(order.Status, changedAtUtc, "payment"));

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Заказ {OrderId} оплачен. PaymentID: {PaymentId}", order.Id, result.PaymentId);
            }
            else
            {
                _logger.LogWarning("Оплата заказа {OrderId} не подтверждена. Статус: {Status}", order.Id, result.Status);

                order.Status = OrderStatus.PaymentFailed;
                order.UpdatedAtUtc = DateTime.UtcNow;
                order.StatusHistory.Add(CreateStatusHistory(order.Status, order.UpdatedAtUtc, "payment"));
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
        catch (TaskCanceledException tce) when (tce.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(tce, "Произошёл timeout оплаты {OrderId}.", order.Id);

            order.Status = OrderStatus.PaymentTimeout;
            order.UpdatedAtUtc = DateTime.UtcNow;
            order.StatusHistory.Add(CreateStatusHistory(order.Status, order.UpdatedAtUtc, "payment"));
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка оплаты {OrderId}.", order.Id);

            order.Status = OrderStatus.Undefined;
            order.UpdatedAtUtc = DateTime.UtcNow;
            order.StatusHistory.Add(CreateStatusHistory(order.Status, order.UpdatedAtUtc, "system"));
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        return order;
    }

    public async Task<Order?> FindOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IReadOnlyCollection<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<bool> TryMarkOrderAsPaidAsync(int orderId, string paymentId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null) return false;

        if (order.Status == OrderStatus.Paid)
            return true;

        if (order.Status != OrderStatus.PendingPayment)
            return false;

        try
        {
            var statusResponse = await _combinedPolicy.ExecuteAsync(async (ct) =>
            {
                _logger.LogInformation("Проверка статуса платежа {PaymentId} для заказа {OrderId}...", paymentId, orderId);
                return await _paymentClient.GetPaymentStatusAsync(paymentId);
            }, CancellationToken.None);

            if (statusResponse.Status == ConfirmedPaymentStatus)
            {
                var changedAtUtc = DateTime.UtcNow;

                order.Status = OrderStatus.Paid;
                order.PaymentId = paymentId;
                order.UpdatedAtUtc = changedAtUtc;
                order.StatusHistory.Add(CreateStatusHistory(order.Status, changedAtUtc, "payment"));
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }

            _logger.LogWarning("Статус платежа {PaymentId} не {ConfirmedStatus}: {Status}", paymentId, ConfirmedPaymentStatus, statusResponse.Status);
            return false;
        }
        catch (TaskCanceledException tce) when (tce.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(tce, "Timeout при проверке статуса платежа {PaymentId} для заказа {OrderId}.", paymentId, orderId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке статуса платежа {PaymentId} для заказа {OrderId}.", paymentId, orderId);
            return false;
        }
    }

    public async Task<bool> TryApplyPaymentResultAsync(int orderId, PaymentResponse payment)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null)
        {
            return false;
        }

        if (order.Status == OrderStatus.Paid)
        {
            return true;
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            return false;
        }

        var changedAtUtc = DateTime.UtcNow;

        if (payment.Status == ConfirmedPaymentStatus && !string.IsNullOrWhiteSpace(payment.PaymentId))
        {
            order.Status = OrderStatus.Paid;
            order.PaymentId = payment.PaymentId;
            order.UpdatedAtUtc = changedAtUtc;
            order.StatusHistory.Add(CreateStatusHistory(order.Status, changedAtUtc, "payment"));
        }
        else
        {
            order.Status = OrderStatus.PaymentFailed;
            order.UpdatedAtUtc = changedAtUtc;
            order.StatusHistory.Add(CreateStatusHistory(order.Status, changedAtUtc, "payment"));
        }

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return order.Status == OrderStatus.Paid;
    }

    public async Task<bool> TryCancelOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null)
        {
            _logger.LogWarning("Попытка отменить несуществующий заказ с ID {OrderId}.", orderId);
            return false;
        }

        if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
        {
            _logger.LogWarning("Попытка отменить заказ {OrderId} с уже финальным статусом {Status}.", orderId, order.Status);
            return false;
        }


        var now = DateTime.UtcNow;
        order.Status = OrderStatus.Cancelled;
        order.UpdatedAtUtc = now;
        order.StatusHistory.Add(new Domain.OrderStatusHistory
        {
            Status = order.Status,
            ChangedAtUtc = now,
            ChangedBy = "user"
        });

        try
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Заказ {OrderId} успешно отменён.", orderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отмене заказа {OrderId}.", orderId);
            return false;
        }
    }

    public async Task<bool> TryMarkOrderAsDeliveredAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null)
        {
            _logger.LogWarning("Попытка отметить доставку несуществующего заказа с ID {OrderId}.", orderId);
            return false;
        }

        if (order.Status != OrderStatus.Paid && order.Status != OrderStatus.Preparing && order.Status != OrderStatus.Delivering)
        {
            _logger.LogWarning("Попытка отметить доставку заказа {OrderId} с недопустимым статусом {Status}.", orderId, order.Status);
            return false;
        }

        var now = DateTime.UtcNow;
        order.Status = OrderStatus.Delivered;
        order.UpdatedAtUtc = now;
        order.StatusHistory.Add(new Domain.OrderStatusHistory
        {
            Status = order.Status,
            ChangedAtUtc = now,
            ChangedBy = "system"
        });

        try
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Заказ {OrderId} успешно отмечен как доставленный (Completed).", orderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отметке доставки заказа {OrderId}.", orderId);
            return false;
        }
    }

    private static OrderStatusHistory CreateStatusHistory(OrderStatus status, DateTime changedAtUtc, string changedBy)
    {
        return new OrderStatusHistory
        {
            Status = status,
            ChangedAtUtc = changedAtUtc,
            ChangedBy = changedBy
        };
    }
}
