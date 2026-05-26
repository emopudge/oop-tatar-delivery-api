using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Clients;

public class MockTinkoffPaymentClient : IPaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MockTinkoffPaymentClient> _logger;
    private readonly string _mockBaseUrl;

    public MockTinkoffPaymentClient(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MockTinkoffPaymentClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("TinkoffMock");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mockBaseUrl = configuration["Tinkoff:MockBaseUrl"] ?? "http://localhost:5001/api/tinkoff/mock";

        _httpClient.BaseAddress = new Uri(_mockBaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        _logger.LogInformation("Создаём mock для заказа №{OrderId}...", request.OrderId);

        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.PostAsync("/create", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponse>(responseJson)
                ?? throw new InvalidOperationException("Неподходящий ответ для мока.");

            _logger.LogInformation("Оплата создана: ID={PaymentId}, Status={Status}", result.PaymentId, result.Status);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP во время создания OrderId: {OrderId}", request.OrderId);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error for payment response (OrderId: {OrderId})", request.OrderId);
            throw;
        }
    }

    public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
    {
        _logger.LogInformation("Ожидаем статус оплаты для PaymentId: {PaymentId}", paymentId);

        try
        {
            using var response = await _httpClient.GetAsync($"/status/{paymentId}");
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponse>(responseJson)
                ?? throw new InvalidOperationException("Неподходящий ответ для мока.");

            _logger.LogInformation("Статус оплаты: ID={PaymentId}, Status={Status}", result.PaymentId, result.Status);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP во время создания PaymentId: {PaymentId}", paymentId);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error for payment status (PaymentId: {PaymentId})", paymentId);
            throw;
        }
    }
}