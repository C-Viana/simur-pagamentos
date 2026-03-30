using simur_backend.Models.DTO.V1;

namespace simur_backend.Services.Customers
{
    public interface ICustomerServices
    {
        Task<CustomerDto> CreateCustomerAsync(CustomerDto customer);
        Task<CustomerDto> FindCustomerByDocumentAsync(string document);
        Task<CustomerDto> FindCustomerByIdAsync(string id);
        Task<CustomerDto> UpdateCustomerAsync(CustomerDto currentCustomer, CustomerDto customer);
        Task<bool> DeleteCustomerByDocumentAsync(string document);

    }
}
