using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Services.Payments
{
    public interface IPaymentServices
    {
        Task<PaymentDto> CreateAsync(PaymentDto payment);
        Task<PaymentDto> UpdateAsync(PaymentStatusHistory paymentStatus);
        Task<PaymentDto> DeleteAsync(Guid paymentId);
        Task<PaymentDto> FindByIdAsync(Guid paymentId);
        Task<PaymentDto> FindByExternalOrderIdAsync(string externalOrderId);
        Task<List<PaymentDto>> FindByMerchantDocAsync(string merchantId);
        Task<List<PaymentDto>> FindByCustomerDocAsync(string CustomerId);
        Task<List<PaymentDto>> FindByCreatedAtAsync(DateOnly paymentDate);

        Task<Models.Entities.PaymentMethod> FindDetailsByIdAsync(long id);
        Task<Models.Entities.PaymentMethod> FindDetailsByPaymentIdAsync(Guid PaymentId);
        Task<List<Models.Entities.PaymentMethod>> FindDetailsByPaymentTypeAsync(Models.Constants.PaymentType type);
        Task<Models.Entities.PaymentMethod> CreatePaymentDetailsAsync(Models.Entities.PaymentMethod paymentMethodDetails);
        Task<Models.Entities.PaymentMethod> UpdatePaymentDetailsAsync(Models.Entities.PaymentMethod paymentMethodDetails);
        Task<Models.Entities.PaymentMethod> DeletePaymentDetailsAsync(long id);
    }
}
