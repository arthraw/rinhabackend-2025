using rinhabackend.Application.DTOs;

namespace rinhabackend.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> CreatePayment(PaymentRequestDto paymentDto, string url);
}