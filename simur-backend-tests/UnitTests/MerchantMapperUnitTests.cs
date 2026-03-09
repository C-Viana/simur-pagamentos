using FluentAssertions;
using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend_tests.UnitTests
{
    public class MerchantMapperUnitTests
    {
        private readonly MerchantConverter _mapper;

        public MerchantMapperUnitTests()
        {
            _mapper = new MerchantConverter();
        }

        private Merchant getTestMerchant(Guid id)
        {
            return new Merchant(
                (id == Guid.Empty) ? Guid.NewGuid() : id,
                "Analu e Ayla Adega ME",
                "Adega Analu e Ayla",
                "53178402000147",
                "contato@analueaylaadegame.com.br",
                "11997672338",
                new Address(),
                "53178402000147",
                "0102953"
            );
        }

        private MerchantDto getTestMerchantDto(Guid id)
        {
            return new MerchantDto()
            {
                Id = (id == Guid.Empty) ? Guid.NewGuid() : id,
                CompanyName = "Analu e Ayla Adega ME",
                TradeName = "Adega Analu e Ayla",
                Document = "53178402000147",
                Email = "contato@analueaylaadegame.com.br",
                PhoneNumber = "11997672338",
                Address = new Address(),
                PixKey = "53178402000147",
                BankAccountId = "0102953"
            };
        }

        [Fact(DisplayName = "Should parse Merchant from entity to DTO successfully")]
        public void Parse_ConvertMerchantFromEntityToDto_Success()
        {
            Guid _id = Guid.NewGuid();
            Merchant entity = getTestMerchant(_id);
            MerchantDto dto = getTestMerchantDto(_id);

            MerchantDto? result = _mapper.Parse( entity );
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dto);
        }

        [Fact(DisplayName = "Should parse Merchant from DTO to entity successfully")]
        public void Parse_ConvertMerchantFromDtoToEntity_Success()
        {
            Guid _id = Guid.NewGuid();
            Merchant entity = getTestMerchant(_id);
            MerchantDto dto = getTestMerchantDto(_id);

            Merchant? result = _mapper.Parse(dto);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(entity);
        }

        [Fact(DisplayName = "Should result into a NULL DTO when converting a NULL entity")]
        public void Parse_ConvertNulledEntity()
        {
            Guid _id = Guid.NewGuid();
            Merchant? entity = null;
            MerchantDto? dto = getTestMerchantDto(_id);

            MerchantDto? result = _mapper.Parse(entity);
            result.Should().BeNull();
            result.Should().NotBe(dto);
        }

        [Fact(DisplayName = "Should result into a NULL entity when converting a NULL DTO")]
        public void Parse_ConvertNulledDto()
        {
            Guid _id = Guid.NewGuid();
            Merchant? entity = getTestMerchant(_id);
            MerchantDto? dto = null;

            Merchant? result = _mapper.Parse(dto);
            result.Should().BeNull();
            result.Should().NotBe(entity);
        }
        
    }
}