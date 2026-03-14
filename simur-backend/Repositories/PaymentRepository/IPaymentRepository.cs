using MongoDB.Driver;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(IClientSessionHandle session, Payment payment);
        Task<Payment> UpdateAsync(IClientSessionHandle session, Payment payment);
        Task<Payment> DeleteAsync(Guid id);
        Task<Payment> FindByIdAsync(IClientSessionHandle session, Guid id);
        Task<Payment> FindByIdAsync(Guid id);
        Task<Payment> FindByExternalOrderIdAsync(string id);
        Task<List<Payment>> FindByMerchantDocAsync(string id);
        Task<List<Payment>> FindByCustomerDocAsync(string id);
        Task<List<Payment>> FindByCreatedAtAsync(DateOnly date);
    }
}
