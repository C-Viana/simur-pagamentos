using MongoDB.Driver;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IMongoCollection<Models.Entities.PaymentMethod> _collection;

        public PaymentMethodRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Models.Entities.PaymentMethod>("payment_methods");
        }

        public async Task<Models.Entities.PaymentMethod> CreateAsync(Models.Entities.PaymentMethod methodDetails)
        {
            await _collection.InsertOneAsync(methodDetails);
            return await _collection.Find(entity => entity.PaymentId == methodDetails.PaymentId).FirstOrDefaultAsync();
        }

        public async Task<Models.Entities.PaymentMethod> DeleteAsync(long id)
        {
            FilterDefinition<Models.Entities.PaymentMethod> filter = Builders<Models.Entities.PaymentMethod>.Filter.Eq(entity => entity.Id, id);
            Models.Entities.PaymentMethod DeletedMethod = await _collection.FindOneAndDeleteAsync(filter);
            return DeletedMethod;
        }

        public async Task<Models.Entities.PaymentMethod> DeleteByPaymentIdAsync(Guid paymentId)
        {
            FilterDefinition<Models.Entities.PaymentMethod> filter = Builders<Models.Entities.PaymentMethod>.Filter.Eq(entity => entity.PaymentId, paymentId);
            Models.Entities.PaymentMethod DeletedMethod = await _collection.FindOneAndDeleteAsync(filter);
            return DeletedMethod;
        }

        public async Task<Models.Entities.PaymentMethod> FindByIdAsync(long id)
        {
            FilterDefinition<Models.Entities.PaymentMethod> filter = Builders<Models.Entities.PaymentMethod>.Filter.Eq(entity => entity.Id, id);
            Models.Entities.PaymentMethod FoundMethod = await _collection.Find(filter).FirstOrDefaultAsync();
            return FoundMethod;
        }

        public async Task<Models.Entities.PaymentMethod> FindByPaymentAsync(Guid paymentId)
        {
            FilterDefinition<Models.Entities.PaymentMethod> filter = Builders<Models.Entities.PaymentMethod>.Filter.Eq(entity => entity.PaymentId, paymentId);
            Models.Entities.PaymentMethod FoundMethod = await _collection.Find(filter).FirstOrDefaultAsync();
            return FoundMethod;
        }

        public async Task<List<Models.Entities.PaymentMethod>> FindByPaymentTypeAsync(Models.Constants.PaymentType type)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.PaymentType, type);
            List<Models.Entities.PaymentMethod> FoundMethods = await _collection.Find((FilterDefinition<Models.Entities.PaymentMethod>)filter).ToListAsync();
            return FoundMethods;
        }

        public async Task<Models.Entities.PaymentMethod> UpdateAsync(Models.Entities.PaymentMethod methodDetailsUpdate)
        {
            FilterDefinition<Models.Entities.PaymentMethod> filter = Builders<Models.Entities.PaymentMethod>.Filter.Eq(entity => entity.Id, methodDetailsUpdate.Id);
            await _collection.ReplaceOneAsync(filter, methodDetailsUpdate);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
