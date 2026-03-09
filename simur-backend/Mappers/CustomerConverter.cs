using rest_with_asp_net_10_cviana.Data.Converter.Contract;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using System.Numerics;
using System.Reflection.Metadata;

namespace simur_backend.Mappers
{
    public class CustomerConverter : IParser<Customer, CustomerDto>, IParser<CustomerDto, Customer>
    {
        public CustomerDto? Parse(Customer entity)
        {
            if (entity == null) return null;
            return new CustomerDto
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Document = entity.Document,
                Email = entity.Email,
                Phone = entity.Phone,
                Birthdate = entity.Birthdate,
                Address = entity.Address,
                ExternalBuyerId = entity.ExternalBuyerId
            };
        }

        public Customer? Parse(CustomerDto dto)
        {
            if (dto == null) return null;
            return new Customer
            (
                dto.Id,
                dto.FullName,
                dto.Document,
                dto.Email,
                dto.Phone,
                dto.Birthdate,
                dto.Address,
                dto.ExternalBuyerId
            );
        }

        public List<CustomerDto?> ParseList(List<Customer> origin)
        {
            if (origin == null) return [];
            return [.. origin.Select(Parse)];
        }

        public List<Customer?> ParseList(List<CustomerDto> origin)
        {
            if (origin == null) return [];
            return [.. origin.Select(Parse)];
        }
    }
}
