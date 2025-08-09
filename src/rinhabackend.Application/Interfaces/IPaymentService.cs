using rinhabackend.Application.DTOs;

namespace rinhabackend.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> createPayment(PaymentDto paymentDto);
}