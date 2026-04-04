using MongoDB.Driver;
using simur_backend.Exceptions.CustomExceptions;
using simur_backend.Mappers;
using simur_backend.Messaging;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;
using simur_backend.Models.Pagination;
using simur_backend.Repositories.PaymentRepository;

namespace simur_backend.Services.Payments
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentStatusHistoryRepository _statusHistoryRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IMerchantServices _merchantService;
        private readonly PaymentConverter _mapper;
        private readonly ILogger<PaymentServices> _logger;
        private readonly IMongoClient _mongoClient;
        private readonly IMessageBusService _broker;

        public PaymentServices(IPaymentRepository paymentRepository, IPaymentStatusHistoryRepository statusHistoryRepository, IPaymentMethodRepository paymentMethodRepository, ILogger<PaymentServices> logger, IMongoClient mongoClient, IMerchantServices merchantService, IMessageBusService broker)
        {
            _mapper = new PaymentConverter();
            _paymentRepository = paymentRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _logger = logger;
            _mongoClient = mongoClient;
            _merchantService = merchantService;
            _broker = broker;
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
                MerchantDto merchant = null;

                switch (payment.PaymentDetails.PaymentType)
                {
                    case PaymentType.BOLETO:
                        payment.PaymentDetails = ((BoletoDetails)payment.PaymentDetails).GenerateSlipCodes(CreatedPayment.Id, CreatedPayment.Amount, context.Request);
                        break;
                    case PaymentType.PIX_DYNAMIC:
                        merchant = await _merchantService.FindMerchantByDocumentAsync(payment.SellerDocument);
                        if (merchant == null) throw new BadHttpRequestException($"No merchant with document {payment.SellerDocument} was found");
                        payment.PaymentDetails = ((PixDynamicDetails)payment.PaymentDetails).GenerateDynamicPixPayment(CreatedPayment.Id, merchant, CreatedPayment.Amount, context.Request);
                        break;
                    case PaymentType.PIX_STATIC:
                        merchant = await _merchantService.FindMerchantByDocumentAsync(payment.SellerDocument);
                        if (merchant == null) throw new BadHttpRequestException($"No merchant with document {payment.SellerDocument} was found");
                        payment.PaymentDetails = ((PixStaticDetails)payment.PaymentDetails).GenerateStaticPixPayment(CreatedPayment.Id, merchant, CreatedPayment.Amount, context.Request);
                        break;
                    case PaymentType.CREDIT_CARD:
                        payment.PaymentDetails = ((CreditCardDetails)payment.PaymentDetails);
                        break;
                    case PaymentType.DEBIT_CARD:
                        payment.PaymentDetails = ((DebitCardDetails)payment.PaymentDetails);
                        break;
                    default:
                        payment.PaymentDetails = payment.PaymentDetails;
                        break;
                }

                PaymentMethod SavedMethod = await _paymentMethodRepository.CreateAsync(_sessionHandle, new PaymentMethod(CreatedPayment.Id, payment.PaymentDetails.PaymentType, payment.PaymentDetails));
                _logger.LogInformation("Finished creating {method} payment from order {order}", payment.PaymentDetails.PaymentType, payment.ExternalOrderId);

                PaymentStatusHistory paymentStatusUpdate = new(CreatedPayment.Id, SavedMethod.PaymentType, CreatedPayment.Status, "Pagamento registrado no sistema", NewPayment.CreatedAt);
                await _statusHistoryRepository.CreateHistoryInfoAsync(_sessionHandle, paymentStatusUpdate);
                _logger.LogInformation("Finished creating status entry {status} for payment from order {order}", paymentStatusUpdate.Status, payment.ExternalOrderId);
                await _broker.PublishPaymentStatus(paymentStatusUpdate);

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

        public async Task<List<PaymentDto>> FindByMerchantDocAsync(string merchantDoc)
        {
            List<Payment> PaymentsFound = await _paymentRepository.FindByMerchantDocAsync(merchantDoc);
            return _mapper.ParseList(PaymentsFound);
        }

        public async Task<PagedResponse<PaymentDto>> FindByCreatedAtAsync(DateOnly paymentDate, int pageNumber, int pageSize, string sortDirection)
        {
            PagedResponse<Payment> PaymentsFound = await _paymentRepository.FindByCreatedAtAsync(paymentDate, pageNumber, pageSize, sortDirection);
            PagedResponse<PaymentDto> response = new PagedResponse<PaymentDto>(_mapper.ParseList(PaymentsFound.Items), PaymentsFound.TotalCount, PaymentsFound.CurrentPage, PaymentsFound.PageSize);
            return response;
        }

        public async Task<PagedResponse<PaymentDto>> FindByCustomerDocAsync(string CustomerDoc, int pageNumber, int pageSize, string sortDirection)
        {
            PagedResponse<Payment> PaymentsFound = await _paymentRepository.FindByCustomerDocAsync(CustomerDoc, pageNumber, pageSize, sortDirection);
            PagedResponse<PaymentDto> response = new PagedResponse<PaymentDto>(_mapper.ParseList(PaymentsFound.Items), PaymentsFound.TotalCount, PaymentsFound.CurrentPage, PaymentsFound.PageSize);
            return response;
        }

        public async Task<PagedResponse<PaymentDto>> FindByMerchantDocAsync(string merchantDoc, int pageNumber, int pageSize, string sortDirection)
        {
            PagedResponse<Payment> PaymentsFound = await _paymentRepository.FindByMerchantDocAsync(merchantDoc, pageNumber, pageSize, sortDirection);
            PagedResponse<PaymentDto> response = new PagedResponse<PaymentDto>(_mapper.ParseList(PaymentsFound.Items), PaymentsFound.TotalCount, PaymentsFound.CurrentPage, PaymentsFound.PageSize);
            return response;
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

        public async Task<PaymentDto> UpdatePaymentStatusAsync(PaymentStatusHistory paymentStatus)
        {
            using IClientSessionHandle sessionHandler = await _mongoClient.StartSessionAsync();
            sessionHandler.StartTransaction();
            try
            {
                Payment PaymentFound = await _paymentRepository.FindByIdAsync(sessionHandler, paymentStatus.PaymentId);
                if (PaymentFound == null) throw new PaymentNotFoundException($"No payment was found with ID {paymentStatus.PaymentId}");

                paymentStatus.ChangedAt = DateTimeOffset.Now.DateTime;
                PaymentStatusHistory NewStatus = await _statusHistoryRepository.CreateHistoryInfoAsync(sessionHandler, paymentStatus);

                PaymentFound.Status = paymentStatus.Status;
                PaymentFound.UpdatedAt = paymentStatus.ChangedAt;

                Payment PaymentUpdated = await _paymentRepository.UpdateAsync(sessionHandler, PaymentFound);

                await _broker.PublishPaymentStatus(paymentStatus);
                await sessionHandler.CommitTransactionAsync();
                
                _logger.LogInformation("Payment {paymentId} has been updated to status {status}", PaymentUpdated.Id, paymentStatus.Status);

                PaymentMethod RelatedDetails = await _paymentMethodRepository.FindByPaymentAsync(paymentStatus.PaymentId);
                PaymentDto paymentResponse = _mapper.Parse(PaymentUpdated);
                paymentResponse.PaymentDetails = RelatedDetails.PaymentDetails;


                return paymentResponse;
            }
            catch (Exception ex)
            {
                await sessionHandler.AbortTransactionAsync();
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
                if (PaymentFound == null) throw new PaymentNotFoundException($"No payment was found with ID {payment.Id}");
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
