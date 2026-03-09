using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.CustomerRepository;

namespace simur_backend.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly CustomerConverter _mapper;

        private bool ValidadeCustomerPayloadForUpdate(Customer current, Customer updated)
        {
            bool phoneConformity = true, birthdateConformity = true, buyerIdConformity = true, addressConformity = true, emailConformity = true;

            if (!string.IsNullOrEmpty(current.Phone) && string.IsNullOrEmpty(updated.Phone))
            {
                phoneConformity = false;
            }

            //if (current.Birthdate != null && updated.Birthdate == null)
            //{
            //    birthdateConformity = false;
            //}

            if (!string.IsNullOrEmpty(current.ExternalBuyerId) && string.IsNullOrEmpty(updated.ExternalBuyerId))
            {
                buyerIdConformity = false;
            }

            if (current.Address != null && updated.Address == null)
            {
                addressConformity = false;
            }

            if (!string.IsNullOrEmpty(current.Email) && string.IsNullOrEmpty(updated.Email))
            {
                emailConformity = false;
            }

            return phoneConformity && buyerIdConformity && addressConformity && emailConformity;
        }

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
            _mapper = new();
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customer)
        {
            await _repository.CreateAsync(_mapper.Parse(customer));
            Customer SavedEntity = await _repository.FindCustomerByDocumentAsync(customer.Document);
            return _mapper.Parse(SavedEntity);
        }

        public async Task<CustomerDto> FindCustomerByIdAsync(string id)
        {
            Customer FoundEntity = await _repository.FindCustomerByIdAsync(id);
            return _mapper.Parse(FoundEntity);
        }

        public async Task<CustomerDto> FindCustomerByDocumentAsync(string document)
        {
            Customer FoundEntity = await _repository.FindCustomerByDocumentAsync(document);
            return _mapper.Parse(FoundEntity);
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto currentEntity, CustomerDto updateCustomer)
        {
            Customer CurrentEntity = _mapper.Parse(currentEntity);
            Customer UpdateCustomer = _mapper.Parse(updateCustomer);

            if (CurrentEntity.GetHashCode().CompareTo(UpdateCustomer.GetHashCode()) == 0)
            {
                return _mapper.Parse(CurrentEntity);
            }

            if (!ValidadeCustomerPayloadForUpdate(CurrentEntity, UpdateCustomer))
                return null;

            Customer result = await _repository.UpdateCustomerAsync(UpdateCustomer);
            return _mapper.Parse(result);
        }

        public Task<bool> DeleteCustomerByDocumentAsync(string document)
        {
            return _repository.DeleteCustomerAsync(document);
        }
    }
}
