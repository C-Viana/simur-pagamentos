using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
        Task<Payment> DeleteAsync(Guid id);
        Task<Payment?> FindByIdAsync(Guid id);
        Task<Payment> FindByExternalOrderIdAsync(string id);
        Task<List<Payment>> FindByMerchantDocAsync(string id);
        Task<List<Payment>> FindByCustomerDocAsync(string id);
        Task<List<Payment>> FindByCreatedAtAsync(DateOnly date);
    }
}
