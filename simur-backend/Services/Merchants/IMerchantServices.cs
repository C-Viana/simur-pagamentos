using simur_backend.Models.DTO.V1;

namespace simur_backend.Services
{
    public interface IMerchantServices
    {
        Task<MerchantDto> CreateMerchantAsync(MerchantDto merchant);
        Task<MerchantDto> FindMerchantByDocumentAsync(string document);
        Task<MerchantDto> FindMerchantByIdAsync(string id);
        Task<MerchantDto> UpdateMerchantAsync(MerchantDto currentMerchant, MerchantDto updateMerchant);
        Task<bool> DeleteMerchantByDocumentAsync(string document);

    }
}
