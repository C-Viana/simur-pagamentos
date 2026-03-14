using MongoDB.Driver;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod> CreateAsync(IClientSessionHandle session, PaymentMethod methodDetails);
        Task<PaymentMethod> FindByIdAsync(Guid id);
        Task<PaymentMethod> FindByPaymentAsync(Guid paymentId);
        Task<List<PaymentMethod>> FindByPaymentTypeAsync(PaymentType type);
        Task<PaymentMethod> UpdateAsync(PaymentMethod methodDetailsUpdate);
        Task<PaymentMethod> DeleteAsync(Guid id);
        Task<PaymentMethod> DeleteByPaymentIdAsync(Guid id);
    }
}
