using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using TatarDelivery.OrderService.Clients;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using TatarDelivery.OrderService.Data;
using TatarDelivery.OrderService.Domain;
using System;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IPaymentClient _paymentClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        AppDbContext context,
        IPaymentClient paymentClient,
        ILogger<OrderService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));

        if (order.Items.Count == 0)
            throw new ArgumentException("В заказе должен быть хотя бы что-то.");

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

            var retryPolicy = Polly.Policy
                .HandleResult<PaymentResponse>(r => r.Status != "CONFIRMED")
                .Or<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 500));

            var timeoutPolicy = Polly.Policy.TimeoutAsync<PaymentResponse>(TimeSpan.FromSeconds(10));

            var result = await retryPolicy
                .WrapAsync(timeoutPolicy)
                .ExecuteAsync(async (CancellationToken ct) =>
                {
                    _logger.LogInformation("Пробуем создать оплату для заказа {OrderId}...", order.Id);
                    return await _paymentClient.CreatePaymentAsync(paymentRequest);
                }, CancellationToken.None);

            if (result.Status == "CONFIRMED")
            {
                order.Status = OrderStatus.Paid;
                order.PaymentId = result.PaymentId;
                order.UpdatedAtUtc = DateTime.UtcNow;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Заказ {OrderId} оплачен. PaymentID: {PaymentId}",
                    order.Id, result.PaymentId);
            }
            else
            {
                _logger.LogWarning(
                    "Оплата заказа {OrderId} не подтверждена. Статус: {Status}",
                    order.Id, result.Status);

                order.Status = OrderStatus.PaymentFailed;
                order.UpdatedAtUtc = DateTime.UtcNow;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
        catch (TaskCanceledException tce) when (tce.Message.Contains("timeout"))
        {
            _logger.LogError(tce, "Произошёл timeout оплаты {OrderId}.", order.Id);

            order.Status = OrderStatus.PaymentTimeout;
            order.UpdatedAtUtc = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка оплаты {OrderId}.", order.Id);

            order.Status = OrderStatus.Undefined;
            order.UpdatedAtUtc = DateTime.UtcNow;
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

    public async Task<bool> TryMarkOrderAsPaidAsync(int orderId, string paymentId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null) return false;

        if (order.Status == OrderStatus.Paid)
            return true;

        if (order.Status != OrderStatus.PendingPayment)
            return false;

        var statusResponse = await _paymentClient.GetPaymentStatusAsync(paymentId);
        if (statusResponse.Status != "CONFIRMED")
            return false;

        order.Status = OrderStatus.Paid;
        order.PaymentId = paymentId;
        order.UpdatedAtUtc = DateTime.UtcNow;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return true;
    }
}