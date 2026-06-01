using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TatarDelivery.OrderService.Clients;

public class MockTinkoffPaymentClient : IPaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MockTinkoffPaymentClient> _logger;
    // private readonly string _mockBaseUrl;
    private static readonly ConcurrentDictionary<string, PaymentResponse> _paymentCache = new();
    private readonly bool _useRealHttp;
    private static readonly Random _random = new();

    public MockTinkoffPaymentClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MockTinkoffPaymentClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("TinkoffMock");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _useRealHttp = configuration.GetValue<bool>("Tinkoff:UseRealHttp", false);
        
        if (_useRealHttp)
        {
            var baseUrl = configuration["Tinkoff:MockBaseUrl"] ?? "http://localhost:5001/api/tinkoff/mock";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        _logger.LogInformation("Обработка оплаты для заказа №{OrderId} (UseRealHttp={UseRealHttp})...", 
            request.OrderId, _useRealHttp);

        if (!_useRealHttp)
        {
            return GenerateMockPaymentResponse(request.OrderId);
        }

        try
        {
            var json = JsonSerializer.Serialize(request);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync("/create", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponse>(responseJson)
                ?? throw new InvalidOperationException("Неподходящий ответ от мока.");

            _logger.LogInformation("Оплата создана (реальный режим): ID={PaymentId}, Status={Status}", 
                result.PaymentId, result.Status);
            return result;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "HTTP-запрос не удался, используем fallback-мок для OrderId: {OrderId}", request.OrderId);
            return GenerateMockPaymentResponse(request.OrderId);
        }
    }

    public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
    {
        _logger.LogInformation("Проверка статуса для PaymentId: {PaymentId} (UseRealHttp={UseRealHttp})", 
            paymentId, _useRealHttp);

        if (!_useRealHttp)
        {
            return GenerateMockStatusResponse(paymentId);
        }

        try
        {
            using var response = await _httpClient.GetAsync($"/status/{paymentId}");
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaymentResponse>(responseJson)
                ?? throw new InvalidOperationException("Неподходящий ответ от мока.");

            _logger.LogInformation("Статус оплаты (реальный режим): ID={PaymentId}, Status={Status}", 
                result.PaymentId, result.Status);
            return result;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "HTTP-запрос не удался, используем fallback-мок для PaymentId: {PaymentId}", paymentId);
            return GenerateMockStatusResponse(paymentId);
        }
    }

    private PaymentResponse GenerateMockPaymentResponse(string orderId)
    {
        var success = _random.Next(0, 100) < 90;
        
        var response = new PaymentResponse
        {
            PaymentId = success ? $"mock_pay_{Guid.NewGuid():N}" : null,
            Status = success ? "CONFIRMED" : "DECLINED",
            Message = success 
                ? "Оплата успешно обработана (мок)" 
                : "Платёж отклонён банком (мок)"
        };

        _logger.LogInformation("MOCK: Ответ для заказа {OrderId}: {Status}", orderId, response.Status);
        return response;
    }

    private PaymentResponse GenerateMockStatusResponse(string paymentId)
    {
        var isConfirmed = paymentId?.StartsWith("mock_pay_") == true;
        
        var response = new PaymentResponse
        {
            PaymentId = paymentId,
            Status = isConfirmed ? "CONFIRMED" : "PENDING",
            Message = isConfirmed 
                ? "Платёж подтверждён (мок)" 
                : "Ожидание подтверждения (мок)"
        };

        _logger.LogInformation("MOCK: Статус для {PaymentId}: {Status}", paymentId, response.Status);
        return response;
    }
}