using FluentAssertions;
using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend_tests.UnitTests
{
    public class CustomerMapperUnitTests
    {
        private readonly CustomerConverter _mapper;

        public CustomerMapperUnitTests()
        {
            _mapper = new CustomerConverter();
        }

        private Customer getTestCostumer(Guid id)
        {
            return new Customer
            (
                (id == Guid.Empty) ? Guid.NewGuid() : id,
                "Foo",
                "35079985501",
                "foo@mockmail.com",
                "+5543955551000",
                new DateOnly(),
                new Address(),
                "foomocked"
            );
        }

        private CustomerDto getTestCustomerDto(Guid id)
        {
            return new CustomerDto()
            {
                Id = (id == Guid.Empty) ? Guid.NewGuid() : id,
                FullName = "Foo",
                Document = "35079985501",
                Email = "foo@mockmail.com",
                Phone = "+5543955551000",
                Birthdate = new DateOnly(),
                Address = new Address(),
                ExternalBuyerId = "foomocked"
            };
        }

        [Fact(DisplayName = "Should parse Customer from entity to DTO successfully")]
        public void Parse_ConvertCustomerFromEntityToDto_Success()
        {
            Guid _id = Guid.NewGuid();
            Customer entity = getTestCostumer(_id);
            CustomerDto dto = getTestCustomerDto(_id);

            CustomerDto? result = _mapper.Parse( entity );
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dto);
        }

        [Fact(DisplayName = "Should parse Customer from DTO to entity successfully")]
        public void Parse_ConvertCustomerFromDtoToEntity_Success()
        {
            Guid _id = Guid.NewGuid();
            Customer entity = getTestCostumer(_id);
            CustomerDto dto = getTestCustomerDto(_id);

            Customer? result = _mapper.Parse(dto);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(entity);
        }

        [Fact(DisplayName = "Should result into a NULL DTO when converting a NULL entity")]
        public void Parse_ConvertNulledEntity()
        {
            Guid _id = Guid.NewGuid();
            Customer? entity = null;
            CustomerDto? dto = getTestCustomerDto(_id);

            CustomerDto? result = _mapper.Parse(entity);
            result.Should().BeNull();
            result.Should().NotBe(dto);
        }

        [Fact(DisplayName = "Should result into a NULL entity when converting a NULL DTO")]
        public void Parse_ConvertNulledDto()
        {
            Guid _id = Guid.NewGuid();
            Customer? entity = getTestCostumer(_id);
            CustomerDto? dto = null;

            Customer? result = _mapper.Parse(dto);
            result.Should().BeNull();
            result.Should().NotBe(entity);
        }

    }
}
