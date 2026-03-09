using simur_backend.Models.DTO.V1;

namespace simur_backend.Services
{
    public interface IMerchantService
    {
        Task<MerchantDto> CreateMerchantAsync(MerchantDto merchant);
        Task<MerchantDto?> FindMerchantByDocumentAsync(string document);
        Task<MerchantDto?> FindMerchantByIdAsync(string id);
        Task<MerchantDto> UpdateMerchantAsync(MerchantDto merchant);
        Task<bool> DeleteMerchantByDocumentAsync(string document);

    }
}
