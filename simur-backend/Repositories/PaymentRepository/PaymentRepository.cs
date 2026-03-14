using MongoDB.Driver;
using SharpCompress.Common;
using simur_backend.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace simur_backend.Repositories.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _collection;

        public PaymentRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Payment>("payments");
        }

        public async Task<Payment> CreateAsync(IClientSessionHandle session, Payment payment)
        {
            await _collection.InsertOneAsync(session, payment);
            return await _collection.Find(session, entity => entity.ExternalOrderId == payment.ExternalOrderId).FirstOrDefaultAsync();
        }

        public Task<Payment> DeleteAsync(Guid id)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.Id, id);
            return _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<List<Payment>> FindByCreatedAtAsync(DateOnly createdAt)
        {
            //{ CreatedAt: {$gte: new Date("2026-03-07"), $lt: new Date("2026-03-08")} }
            //CreatedAt : 2026-03-07T13:45:56.130+00:00
            DateTimeOffset TargetDate = new(createdAt, TimeOnly.MinValue, TimeSpan.Zero);
            DateTimeOffset LimitDate = TargetDate.AddDays(1);

            FilterDefinition<Payment> filter = 
                Builders<Payment>.Filter.Gte(entity => entity.CreatedAt, TargetDate)
                &
                Builders<Payment>.Filter.Lt(entity => entity.CreatedAt, LimitDate);

            List<Payment> results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<List<Payment>> FindByCustomerDocAsync(string customerDoc)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.CustomerDocument, customerDoc);
            List<Payment> results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<Payment> FindByExternalOrderIdAsync(string externalId)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.ExternalOrderId, externalId);
            Payment results = await _collection.Find(filter).FirstOrDefaultAsync();
            return results;
        }

        public async Task<Payment> FindByIdAsync(IClientSessionHandle session, Guid id)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.Id, id);
            return await _collection.Find(session, filter).FirstOrDefaultAsync();
        }

        public async Task<Payment> FindByIdAsync(Guid id)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> FindByMerchantDocAsync(string merchantDoc)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.MerchantDocument, merchantDoc);
            List<Payment> results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<Payment> UpdateAsync(IClientSessionHandle session, Payment payment)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.Id, payment.Id);
            await _collection.ReplaceOneAsync(session, filter, payment);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
