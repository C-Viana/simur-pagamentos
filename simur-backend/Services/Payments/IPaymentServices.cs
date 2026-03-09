using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Services.Payments
{
    public interface IPaymentServices
    {
        Task<PaymentDto> CreateAsync(PaymentDto payment);
        Task<PaymentDto> UpdateAsync(PaymentStatusHistory paymentStatus);
        Task<PaymentDto> DeleteAsync(Guid paymentId);
        Task<PaymentDto?> FindByIdAsync(Guid paymentId);
        Task<PaymentDto> FindByExternalOrderIdAsync(string externalOrderId);
        Task<List<PaymentDto>> FindByMerchantDocAsync(string merchantId);
        Task<List<PaymentDto>> FindByCustomerDocAsync(string CustomerId);
        Task<List<PaymentDto>> FindByCreatedAtAsync(DateOnly paymentDate);

        Task<PaymentMethod?> FindDetailsByIdAsync(long id);
        Task<PaymentMethod?> FindDetailsByPaymentIdAsync(Guid PaymentId);
        Task<List<PaymentMethod?>> FindDetailsByPaymentTypeAsync(PaymentType type);
        Task<PaymentMethod> CreatePaymentDetailsAsync(PaymentMethod paymentMethodDetails);
        Task<PaymentMethod> UpdatePaymentDetailsAsync(PaymentMethod paymentMethodDetails);
        Task<PaymentMethod> DeletePaymentDetailsAsync(long id);
    }
}
