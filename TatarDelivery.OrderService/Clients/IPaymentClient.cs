using TatarDelivery.OrderService.Contracts.Requests;
using TatarDelivery.OrderService.Contracts.Responses;
using System.Threading.Tasks;

namespace TatarDelivery.OrderService.Clients;

public interface IPaymentClient
{
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
    Task<PaymentResponse> GetPaymentStatusAsync(string paymentId);
}