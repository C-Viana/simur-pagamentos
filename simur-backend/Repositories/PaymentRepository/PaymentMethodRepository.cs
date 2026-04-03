using MongoDB.Driver;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;
using static System.Collections.Specialized.BitVector32;

namespace simur_backend.Repositories.PaymentRepository
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IMongoCollection<PaymentMethod> _collection;

        public PaymentMethodRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<PaymentMethod>("payment_methods");
        }

        public async Task<PaymentMethod> CreateAsync(IClientSessionHandle session, PaymentMethod methodDetails)
        {
            await _collection.InsertOneAsync(session, methodDetails);
            return await _collection.Find(session, entity => entity.PaymentId == methodDetails.PaymentId).FirstOrDefaultAsync();
        }

        public async Task<PaymentMethod> DeleteAsync(Guid id)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.Id, id);
            PaymentMethod DeletedMethod = await _collection.FindOneAndDeleteAsync(filter);
            return DeletedMethod;
        }

        public async Task<PaymentMethod> DeleteByPaymentIdAsync(Guid paymentId)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.PaymentId, paymentId);
            PaymentMethod DeletedMethod = await _collection.FindOneAndDeleteAsync(filter);
            return DeletedMethod;
        }

        public async Task<PaymentMethod> FindByIdAsync(Guid id)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.Id, id);
            PaymentMethod FoundMethod = await _collection.Find(filter).FirstOrDefaultAsync();
            return FoundMethod;
        }

        public async Task<PaymentMethod> FindByPaymentAsync(Guid paymentId)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.PaymentId, paymentId);
            PaymentMethod FoundMethod = await _collection.Find(filter).FirstOrDefaultAsync();
            return FoundMethod;
        }

        public async Task<List<PaymentMethod>> FindByPaymentTypeAsync(PaymentType type)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.PaymentType, type);
            List<PaymentMethod> FoundMethods = await _collection.Find((FilterDefinition<PaymentMethod>)filter).ToListAsync();
            return FoundMethods;
        }

        public async Task<PaymentMethod> UpdateAsync(PaymentMethod methodDetailsUpdate)
        {
            FilterDefinition<PaymentMethod> filter = Builders<PaymentMethod>.Filter.Eq(entity => entity.Id, methodDetailsUpdate.Id);
            FindOneAndReplaceOptions<PaymentMethod> options = new()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _collection.FindOneAndReplaceAsync(filter, methodDetailsUpdate, options);
        }
    }
}
