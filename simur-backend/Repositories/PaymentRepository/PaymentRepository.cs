using MongoDB.Driver;
using simur_backend.Models.Entities;
using simur_backend.Models.Pagination;

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
            DateTimeOffset TargetDate = new(createdAt, TimeOnly.MinValue, TimeSpan.Zero);
            DateTimeOffset LimitDate = TargetDate.AddDays(1);

            FilterDefinition<Payment> filter = 
                Builders<Payment>.Filter.Gte(entity => entity.CreatedAt, TargetDate)
                &
                Builders<Payment>.Filter.Lt(entity => entity.CreatedAt, LimitDate);

            List<Payment> results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<PagedResponse<Payment>> FindByCreatedAtAsync(DateOnly createdAt, int pageNumber, int pageSize, string sortDirection)
        {
            DateTimeOffset TargetDate = new(createdAt, TimeOnly.MinValue, TimeSpan.Zero);
            DateTimeOffset LimitDate = TargetDate.AddDays(1);

            FilterDefinition<Payment> filter =
                Builders<Payment>.Filter.Gte(entity => entity.CreatedAt, TargetDate)
                &
                Builders<Payment>.Filter.Lt(entity => entity.CreatedAt, LimitDate);

            long count = await _collection.CountDocumentsAsync(filter);
            SortDefinition<Payment> sort = (sortDirection.StartsWith("desc", StringComparison.InvariantCultureIgnoreCase))
                ? Builders<Payment>.Sort.Descending(p => p.CreatedAt)
                : Builders<Payment>.Sort.Ascending(p => p.CreatedAt);

            List<Payment> results = await _collection.Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize) // Pular páginas anteriores
                .Limit(pageSize) // Limitar itens da página atual
                .ToListAsync();

            return new PagedResponse<Payment>(results, (int)count, pageNumber, pageSize);
        }

        public async Task<List<Payment>> FindByCustomerDocAsync(string customerDoc)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.CustomerDocument, customerDoc);
            List<Payment> results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<PagedResponse<Payment>> FindByCustomerDocAsync(string customerDoc, int pageNumber, int pageSize, string sortDirection)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.CustomerDocument, customerDoc);

            long count = await _collection.CountDocumentsAsync(filter);
            SortDefinition<Payment> sort = (sortDirection.StartsWith("desc", StringComparison.InvariantCultureIgnoreCase))
                ? Builders<Payment>.Sort.Descending(p => p.CreatedAt)
                : Builders<Payment>.Sort.Ascending(p => p.CreatedAt);

            List<Payment> results = await _collection.Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResponse<Payment>(results, (int)count, pageNumber, pageSize);
        }

        public async Task<List<Payment>> FindByMerchantDocAsync(string merchantDoc)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.MerchantDocument, merchantDoc);
            List<Payment> results = await _collection.Find(filter).ToListAsync();
            return results;
        }

        public async Task<PagedResponse<Payment>> FindByMerchantDocAsync(string merchantDoc, int pageNumber, int pageSize, string sortDirection)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.MerchantDocument, merchantDoc);
            long count = await _collection.CountDocumentsAsync(filter);
            SortDefinition<Payment> sort = (sortDirection.StartsWith("desc", StringComparison.InvariantCultureIgnoreCase))
                ? Builders<Payment>.Sort.Descending(p => p.CreatedAt)
                : Builders<Payment>.Sort.Ascending(p => p.CreatedAt);

            List<Payment> results = await _collection.Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResponse<Payment>(results, (int)count, pageNumber, pageSize);
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

        public async Task<Payment> UpdateAsync(IClientSessionHandle session, Payment payment)
        {
            FilterDefinition<Payment> filter = Builders<Payment>.Filter.Eq(entity => entity.Id, payment.Id);
            FindOneAndReplaceOptions<Payment> options = new()
            {
                ReturnDocument = ReturnDocument.After
            };
            return await _collection.FindOneAndReplaceAsync(session, filter, payment, options);
        }
    }
}
