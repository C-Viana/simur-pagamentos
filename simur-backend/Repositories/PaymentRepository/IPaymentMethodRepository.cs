using simur_backend.Models.Constants;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public interface IPaymentMethodRepository
    {
        Task<Models.Entities.PaymentMethod> CreateAsync(Models.Entities.PaymentMethod methodDetails);
        Task<Models.Entities.PaymentMethod> FindByIdAsync(long id);
        Task<Models.Entities.PaymentMethod> FindByPaymentAsync(Guid paymentId);
        Task<List<Models.Entities.PaymentMethod>> FindByPaymentTypeAsync(Models.Constants.PaymentType type);
        Task<Models.Entities.PaymentMethod> UpdateAsync(Models.Entities.PaymentMethod methodDetailsUpdate);
        Task<Models.Entities.PaymentMethod> DeleteAsync(long id);
        Task<Models.Entities.PaymentMethod> DeleteByPaymentIdAsync(Guid id);
    }
}
