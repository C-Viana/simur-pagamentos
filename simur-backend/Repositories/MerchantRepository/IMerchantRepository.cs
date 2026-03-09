using simur_backend.Models.Entities;

namespace simur_backend.Repositories.MerchantRepository
{
    public interface IMerchantRepository
    {
        Task CreateAsync(Merchant merchant);
        Task<Merchant> FindMerchantByIdAsync(string id);
        Task<Merchant> FindMerchantByDocumentAsync(string document);
        Task<Merchant> UpdateMerchantAsync(Merchant merchant);
        Task<bool> DeleteMerchantAsync(string document);
    }
}
