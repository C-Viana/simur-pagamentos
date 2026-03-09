using MongoDB.Driver;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public interface IPaymentStatusHistoryRepository
    {
        Task<PaymentStatusHistory> CreateHistoryInfoAsync(PaymentStatusHistory paymentUpdate);
        Task<PaymentStatusHistory> FindHistoryInfoAsync(Guid id);
        Task<List<PaymentStatusHistory>> FindHistoryByPaymentInfoAsync(Guid paymentId);
        Task<PaymentStatusHistory> UpdateHistoryInfoAsync(PaymentStatusHistory paymentUpdate);
        Task<PaymentStatusHistory> DeleteHistoryStepAsync(Guid id);
        Task<long> DeleteAllPaymentHistoryAsync(Guid paymentId);
    }
}
