using simur_backend.Models.Entities;

namespace simur_backend.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task CreateAsync(Customer customer);
        Task<Customer> FindCustomerByIdAsync(string id);
        Task<Customer> FindCustomerByDocumentAsync(string document);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(string document);
    }
}
