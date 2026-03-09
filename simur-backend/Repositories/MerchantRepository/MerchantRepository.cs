using MongoDB.Driver;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.MerchantRepository
{
    public class MerchantRepository : IMerchantRepository
    {
        private readonly IMongoCollection<Merchant> _merchants;

        public MerchantRepository(IMongoDatabase database)
        {
            _merchants = database.GetCollection<Merchant>("merchants");
        }

        public async Task CreateAsync(Merchant merchant)
        {
            await _merchants.InsertOneAsync(merchant);
        }

        public async Task<bool> DeleteMerchantAsync(string document)
        {
            FilterDefinition<Merchant> filterDefinition = Builders<Merchant>.Filter.Eq(entity => entity.Document, document);
            Merchant result = await _merchants.FindOneAndDeleteAsync(filterDefinition);
            return result != null;
        }

        public async Task<Merchant> FindMerchantByDocumentAsync(string document)
        {
            FilterDefinition<Merchant> filterDefinition = Builders<Merchant>.Filter.Eq(entity => entity.Document, document);
            return await _merchants.Find(filterDefinition).FirstOrDefaultAsync();
        }

        public async Task<Merchant> FindMerchantByIdAsync(string id)
        {
            FilterDefinition<Merchant> filterDefinition = Builders<Merchant>.Filter.Eq(entity => entity.Id, Guid.Parse(id));
            return await _merchants.Find(filterDefinition).FirstOrDefaultAsync();
        }

        public async Task<Merchant> UpdateMerchantAsync(Merchant merchant)
        {
            FilterDefinition<Merchant> filterDefinition = Builders<Merchant>.Filter.Eq(entity => entity.Id, merchant.Id);
            await _merchants.ReplaceOneAsync(filterDefinition, merchant);
            return await _merchants.Find(filterDefinition).FirstOrDefaultAsync();
        }
    }
}
