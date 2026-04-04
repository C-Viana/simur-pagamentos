using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Models.Pagination;

namespace simur_backend.Services.Payments
{
    public interface IPaymentServices
    {
        Task<PaymentDto> CreateCompletePaymentAsync(PaymentDto payment, HttpContext context);
        Task<PaymentDto> ReplacePaymentAsync(PaymentDto payment);
        Task<PaymentDto> UpdatePaymentStatusAsync(PaymentStatusHistory paymentStatus);
        Task<PaymentDto> DeleteAsync(Guid paymentId);
        Task<PaymentDto> FindByIdAsync(Guid paymentId);
        Task<PaymentDto> FindByExternalOrderIdAsync(string externalOrderId);
        Task<List<PaymentDto>> FindByMerchantDocAsync(string merchantId);
        Task<List<PaymentDto>> FindByCustomerDocAsync(string CustomerId);
        Task<List<PaymentDto>> FindByCreatedAtAsync(DateOnly paymentDate);
        Task<PagedResponse<PaymentDto>> FindByMerchantDocAsync(string merchantId, int pageNumber, int pageSize, string sortDirection);
        Task<PagedResponse<PaymentDto>> FindByCustomerDocAsync(string CustomerId, int pageNumber, int pageSize, string sortDirection);
        Task<PagedResponse<PaymentDto>> FindByCreatedAtAsync(DateOnly paymentDate, int pageNumber, int pageSize, string sortDirection);

        Task<PaymentMethod> FindDetailsByIdAsync(Guid id);
        Task<PaymentMethod> FindDetailsByPaymentIdAsync(Guid PaymentId);
        Task<List<PaymentMethod>> FindDetailsByPaymentTypeAsync(Models.Constants.PaymentType type);
        Task<PaymentMethod> CreatePaymentDetailsAsync(PaymentMethod paymentMethodDetails);
        Task<PaymentMethod> UpdatePaymentDetailsAsync(PaymentMethod paymentMethodDetails);
        Task<PaymentMethod> DeletePaymentDetailsAsync(Guid id);
    }
}
