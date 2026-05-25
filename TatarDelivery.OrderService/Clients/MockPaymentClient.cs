using Microsoft.Extensions.Logging;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Clients;

public class MockTinkoffPaymentClient : IPaymentClient
{
    private readonly ILogger<MockTinkoffPaymentClient> _logger;
    private static int _counter = 0;

    public MockTinkoffPaymentClient(ILogger<MockTinkoffPaymentClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        _logger.LogInformation("Создаём mock для заказа №{OrderId}...", request.OrderId);

        await Task.Delay(Random.Shared.Next(300, 700));

        var paymentId = $"mock_{Interlocked.Increment(ref _counter)}";
        var response = new PaymentResponse(paymentId, "CONFIRMED");

        _logger.LogInformation("Mock оплаты создан: ID={PaymentId}, status={Status}", response.PaymentId, response.Status);
        return response;
    }

    public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
    {
        _logger.LogInformation("Смотрим статус оплаты {PaymentId}...", paymentId);

        await Task.Delay(Random.Shared.Next(200, 500));
        return new PaymentResponse(paymentId, "CONFIRMED");
    }
}