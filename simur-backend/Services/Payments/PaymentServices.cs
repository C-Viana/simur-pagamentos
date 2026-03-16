using MongoDB.Driver;
using simur_backend.Mappers;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;
using simur_backend.Repositories.PaymentRepository;

namespace simur_backend.Services.Payments
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentStatusHistoryRepository _statusHistoryRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IMerchantService _merchantService;
        private readonly PaymentConverter _mapper;
        private readonly ILogger<PaymentServices> _logger;
        private readonly IMongoClient _mongoClient;

        public PaymentServices(IPaymentRepository paymentRepository, IPaymentStatusHistoryRepository statusHistoryRepository, IPaymentMethodRepository paymentMethodRepository, ILogger<PaymentServices> logger, IMongoClient mongoClient, IMerchantService merchantService)
        {
            _mapper = new PaymentConverter();
            _paymentRepository = paymentRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _logger = logger;
            _mongoClient = mongoClient;
            _merchantService = merchantService;
        }

        public async Task<PaymentDto> CreateCompletePaymentAsync(PaymentDto payment, HttpContext context)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try
            {
                Payment NewPayment = _mapper.Parse(payment);
                NewPayment.CreatedAt = DateTimeOffset.Now.DateTime;
                NewPayment.Status = PaymentStatus.CREATED;
                NewPayment.UpdatedAt = NewPayment.CreatedAt;
                Payment CreatedPayment = await _paymentRepository.CreateAsync(_sessionHandle, NewPayment);
                _logger.LogInformation("Payment created with ID {id}. Carring on with payment details", CreatedPayment.Id.ToString());

                MerchantDto merchant = await _merchantService.FindMerchantByDocumentAsync(payment.SellerDocument);

                switch (payment.PaymentDetails.PaymentType)
                {
                    case PaymentType.BOLETO:
                        payment.PaymentDetails = ((BoletoDetails)payment.PaymentDetails).GenerateSlipCodes(CreatedPayment.Id, CreatedPayment.Amount, context.Request);
                        break;
                    case PaymentType.PIX_DYNAMIC:
                        payment.PaymentDetails = ((PixDynamicDetails)payment.PaymentDetails).GenerateDynamicPixPayment(CreatedPayment.Id, merchant, CreatedPayment.Amount, context.Request);
                        break;
                    case PaymentType.CREDIT_CARD:
                        payment.PaymentDetails = ((CreditCardDetails)payment.PaymentDetails);
                        break;
                    default:
                        payment.PaymentDetails = payment.PaymentDetails;
                        break;
                }

                PaymentMethod SavedMethod = await _paymentMethodRepository.CreateAsync(_sessionHandle, new PaymentMethod(CreatedPayment.Id, payment.PaymentDetails.PaymentType, payment.PaymentDetails));
                _logger.LogInformation("Finished creating {method} payment for order {order}", payment.PaymentDetails.PaymentType, payment.ExternalOrderId);

                PaymentStatusHistory paymentUpdate = new(CreatedPayment.Id, CreatedPayment.Status, null, NewPayment.CreatedAt);
                await _statusHistoryRepository.CreateHistoryInfoAsync(_sessionHandle, paymentUpdate);

                await _sessionHandle.CommitTransactionAsync();

                PaymentDto paymentResponse = _mapper.Parse(CreatedPayment);
                paymentResponse.PaymentDetails = SavedMethod.PaymentDetails;
                return paymentResponse;
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Payment transaction failed");
                throw;
            }
        }

        public async Task<PaymentDto> CreateAsync(PaymentDto payment)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try
            {
                Payment NewPayment = _mapper.Parse(payment);
                NewPayment.CreatedAt = DateTimeOffset.Now.DateTime;
                NewPayment.Status = PaymentStatus.CREATED;
                NewPayment.UpdatedAt = NewPayment.CreatedAt;
                Payment CreatedPayment = await _paymentRepository.CreateAsync(_sessionHandle, NewPayment);

                PaymentStatusHistory paymentUpdate = new(CreatedPayment.Id, CreatedPayment.Status, null, NewPayment.CreatedAt);
                await _statusHistoryRepository.CreateHistoryInfoAsync(_sessionHandle, paymentUpdate);
                await _sessionHandle.CommitTransactionAsync();

                return _mapper.Parse(CreatedPayment);
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Payment transaction failed");
                throw;
            }
        }

        public async Task<PaymentDto> DeleteAsync(Guid paymentId)
        {
            Payment DeletedPayment = await _paymentRepository.DeleteAsync(paymentId);
            if (DeletedPayment != null)
            {
                await _statusHistoryRepository.DeleteAllPaymentHistoryAsync(paymentId);
                await _paymentMethodRepository.DeleteByPaymentIdAsync(paymentId);
            }
            return _mapper.Parse(DeletedPayment);
        }

        public async Task<List<PaymentDto>> FindByCreatedAtAsync(DateOnly paymentDate)
        {
            List<Payment> PaymentsFound = await _paymentRepository.FindByCreatedAtAsync(paymentDate);
            return _mapper.ParseList(PaymentsFound);
        }

        public async Task<List<PaymentDto>> FindByCustomerDocAsync(string CustomerDoc)
        {
            List<Payment> PaymentsFound = await _paymentRepository.FindByCustomerDocAsync(CustomerDoc);
            return _mapper.ParseList(PaymentsFound);
        }

        public async Task<PaymentDto> FindByExternalOrderIdAsync(string externalOrderId)
        {
            Payment PaymentsFound = await _paymentRepository.FindByExternalOrderIdAsync(externalOrderId);
            return _mapper.Parse(PaymentsFound);
        }

        public async Task<PaymentDto> FindByIdAsync(Guid paymentId)
        {
            Payment PaymentFound = await _paymentRepository.FindByIdAsync(paymentId);
            return _mapper.Parse(PaymentFound);
        }

        public async Task<List<PaymentDto>> FindByMerchantDocAsync(string merchantDoc)
        {
            List<Payment> PaymentsFound = await _paymentRepository.FindByMerchantDocAsync(merchantDoc);
            return _mapper.ParseList(PaymentsFound);
        }

        public async Task<PaymentDto> UpdatePaymentStatusAsync(PaymentStatusHistory paymentStatus)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try
            {
                Payment PaymentFound = await _paymentRepository.FindByIdAsync(_sessionHandle, paymentStatus.PaymentId);
                if (PaymentFound == null) throw new ArgumentNullException(nameof(PaymentFound));

                paymentStatus.ChangedAt = DateTimeOffset.Now.DateTime;
                PaymentStatusHistory NewStatus = await _statusHistoryRepository.CreateHistoryInfoAsync(_sessionHandle, paymentStatus);

                PaymentFound.Status = paymentStatus.Status;
                PaymentFound.UpdatedAt = paymentStatus.ChangedAt;

                Payment PaymentUpdated = await _paymentRepository.UpdateAsync(_sessionHandle, PaymentFound);

                await _sessionHandle.CommitTransactionAsync();
                
                _logger.LogInformation("Payment {paymentId} has been updated to status {status}", PaymentUpdated.Id, PaymentUpdated.Status);

                return _mapper.Parse(PaymentUpdated);
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Payment transaction failed. Update aborted");
                throw;
            }
        }

        public async Task<PaymentMethod> FindDetailsByIdAsync(Guid id) {
            return await _paymentMethodRepository.FindByIdAsync(id);
        }

        public async Task<PaymentMethod> FindDetailsByPaymentIdAsync(Guid PaymentId) {
            return await _paymentMethodRepository.FindByPaymentAsync(PaymentId);
        }

        public async Task<List<PaymentMethod>> FindDetailsByPaymentTypeAsync(PaymentType type) {
            return await _paymentMethodRepository.FindByPaymentTypeAsync(type);
        }

        public async Task<PaymentMethod> CreatePaymentDetailsAsync(PaymentMethod paymentMethodDetails)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try
            {
                PaymentMethod CreatedMethod = await _paymentMethodRepository.CreateAsync(_sessionHandle, paymentMethodDetails);
                await _sessionHandle.CommitTransactionAsync();
                return CreatedMethod;
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Payment transaction failed");
                throw;
            }
        }

        public async Task<PaymentMethod> UpdatePaymentDetailsAsync(PaymentMethod paymentMethodDetailsUpdate) {
            await _paymentMethodRepository.UpdateAsync(paymentMethodDetailsUpdate);
            return await _paymentMethodRepository.FindByIdAsync(paymentMethodDetailsUpdate.Id);
        }

        public async Task<PaymentMethod> DeletePaymentDetailsAsync(Guid id) {
            return await _paymentMethodRepository.DeleteAsync(id);
        }

        public async Task<PaymentDto> ReplacePaymentAsync(PaymentDto payment)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try{
                Payment PaymentFound = await _paymentRepository.FindByIdAsync(payment.Id);
                if (PaymentFound == null) throw new ArgumentNullException(nameof(PaymentFound));
                Payment PaymentUpdated = await _paymentRepository.UpdateAsync(_sessionHandle, _mapper.Parse(payment));
                await _sessionHandle.CommitTransactionAsync();
                return _mapper.Parse(PaymentUpdated);
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Payment transaction failed");
                throw;
            }
        }
    }
}
