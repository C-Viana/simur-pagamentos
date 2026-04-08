using MongoDB.Driver;
using simur_backend.Models.Entities;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Repositories.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerRepository(IMongoDatabase database)
        {
            _customers = database.GetCollection<Customer>("customers");
        }

        public async Task CreateAsync(Customer customer)
        {
            await _customers.InsertOneAsync(customer);
        }

        public async Task<Customer> FindCustomerByIdAsync(string id)
        {
            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(entity => entity.Id, Guid.Parse(id));
            return await _customers.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Customer> FindCustomerByDocumentAsync(string document)
        {
            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(entity => entity.Document, document);
            return await _customers.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(entity => entity.Id, customer.Id);
            FindOneAndReplaceOptions<Customer> options = new()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _customers.FindOneAndReplaceAsync(filter, customer, options);
        }

        public async Task<bool> DeleteCustomerAsync(string document)
        {
            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(entity => entity.Document, document);
            Customer result = await _customers.FindOneAndDeleteAsync(filter);
            return result != null;
        }
    }
}
