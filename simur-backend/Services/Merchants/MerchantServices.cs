using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.MerchantRepository;

namespace simur_backend.Services.Merchants
{
    public class MerchantServices : IMerchantServices
    {
        private readonly IMerchantRepository _repository;
        private readonly MerchantConverter _mapper;

        public MerchantServices(IMerchantRepository repository)
        {
            _repository = repository;
            _mapper = new();
        }

        public async Task<MerchantDto> CreateMerchantAsync(MerchantDto merchant)
        {
            await _repository.CreateAsync(_mapper.Parse(merchant));
            Merchant SavedEntity = await _repository.FindMerchantByDocumentAsync(merchant.Document);
            return _mapper.Parse(SavedEntity);
        }

        public async Task<MerchantDto> FindMerchantByIdAsync(string id)
        {
            Merchant FoundEntity = await _repository.FindMerchantByIdAsync(id);
            return _mapper.Parse(FoundEntity);
        }

        public async Task<MerchantDto> FindMerchantByDocumentAsync(string document)
        {
            Merchant FoundEntity = await _repository.FindMerchantByDocumentAsync(document);
            return _mapper.Parse(FoundEntity);
        }

        public async Task<MerchantDto> UpdateMerchantAsync(MerchantDto currentMerchant, MerchantDto updateMerchant)
        {
            Merchant CurrentEntity = _mapper.Parse(currentMerchant);
            Merchant UpdateMerchant = _mapper.Parse(updateMerchant);

            if( CurrentEntity.GetHashCode().CompareTo(UpdateMerchant.GetHashCode()) == 0 )
            {
                return _mapper.Parse(CurrentEntity);
            }

            Merchant result = await _repository.UpdateMerchantAsync(UpdateMerchant);
            return _mapper.Parse(result);
        }

        public Task<bool> DeleteMerchantByDocumentAsync(string document)
        {
            return _repository.DeleteMerchantAsync(document);
        }
    }
}
