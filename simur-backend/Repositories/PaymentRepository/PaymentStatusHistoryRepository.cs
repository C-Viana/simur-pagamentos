using MongoDB.Driver;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.PaymentRepository
{
    public class PaymentStatusHistoryRepository : IPaymentStatusHistoryRepository
    {
        private readonly IMongoCollection<PaymentStatusHistory> _collection;

        public PaymentStatusHistoryRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<PaymentStatusHistory>("payment_status_history");
        }

        public async Task<PaymentStatusHistory> CreateHistoryInfoAsync(IClientSessionHandle session, PaymentStatusHistory paymentUpdate)
        {
            await _collection.InsertOneAsync(session, paymentUpdate);
            return await _collection.Find(session, entity => entity.PaymentId == paymentUpdate.PaymentId).FirstOrDefaultAsync();
        }

        public async Task<long> DeleteAllPaymentHistoryAsync(Guid paymentId)
        {
            FilterDefinition<PaymentStatusHistory> filter = Builders<PaymentStatusHistory>.Filter.Eq(entity => entity.PaymentId, paymentId);
            DeleteResult result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<PaymentStatusHistory> DeleteHistoryStepAsync(Guid id)
        {
            FilterDefinition<PaymentStatusHistory> filter = Builders<PaymentStatusHistory>.Filter.Eq(entity => entity.Id, id);
            PaymentStatusHistory result = await _collection.FindOneAndDeleteAsync(filter);
            return result;
        }

        public async Task<List<PaymentStatusHistory>> FindHistoryByPaymentInfoAsync(Guid paymentId)
        {
            FilterDefinition<PaymentStatusHistory> filter = Builders<PaymentStatusHistory>.Filter.Eq(entity => entity.PaymentId, paymentId);
            List<PaymentStatusHistory> result = await _collection.Find(filter).ToListAsync();
            if (result.Count == 0) return [];
            else return result;
        }

        public async Task<PaymentStatusHistory> FindHistoryInfoAsync(Guid id)
        {
            FilterDefinition<PaymentStatusHistory> filter = Builders<PaymentStatusHistory>.Filter.Eq(entity => entity.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<PaymentStatusHistory> UpdateHistoryInfoAsync(PaymentStatusHistory paymentUpdate)
        {
            FilterDefinition<PaymentStatusHistory> filter = Builders<PaymentStatusHistory>.Filter.Eq(entity => entity.Id, paymentUpdate.Id);
            await _collection.ReplaceOneAsync(filter, paymentUpdate);
            return _collection.Find(filter).FirstOrDefault();
        }
    }
}
