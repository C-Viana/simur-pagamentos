using simur_backend.Models.Constants;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod> CreateAsync(PaymentMethod methodDetails);
        Task<PaymentMethod?> FindByIdAsync(long id);
        Task<PaymentMethod> FindByPaymentAsync(Guid paymentId);
        Task<List<PaymentMethod?>> FindByPaymentTypeAsync(PaymentType type);
        Task<PaymentMethod> UpdateAsync(PaymentMethod methodDetailsUpdate);
        Task<PaymentMethod> DeleteAsync(long id);
        Task<PaymentMethod> DeleteByPaymentIdAsync(Guid id);
    }
}
