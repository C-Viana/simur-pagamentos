using rest_with_asp_net_10_cviana.Data.Converter.Contract;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Mappers
{
    public class MerchantConverter : IParser<Merchant, MerchantDto>, IParser<MerchantDto, Merchant>
    {
        public MerchantDto? Parse(Merchant origin)
        {
            if(origin == null) return null;
            return new MerchantDto()
            {
                Id = origin.Id,
                CompanyName = origin.CompanyName,
                TradeName = origin.TradeName,
                Document = origin.Document,
                Email = origin.Email,
                PhoneNumber = origin.PhoneNumber,
                Address = origin.Address,
                PixKey = origin.PixKey,
                BankAccountId = origin.BankAccountId
            };
        }

        public List<MerchantDto?> ParseList(List<Merchant> origin)
        {
            return [.. origin.Select(Parse)];
        }
        public Merchant? Parse(MerchantDto dto)
        {
            if (dto == null) return null;
            return new Merchant
            (
                dto.Id,
                dto.CompanyName,
                dto.TradeName,
                dto.Document,
                dto.Email,
                dto.PhoneNumber,
                dto.Address,
                dto.PixKey,
                dto.BankAccountId
            );
        }

        public List<Merchant?> ParseList(List<MerchantDto> dto)
        {
            return [.. dto.Select(Parse)];
        }
    }
}
