using simur_backend.Mappers;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.PaymentRepository;

namespace simur_backend.Services.Payments
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentStatusHistoryRepository _statusHistoryRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly PaymentConverter _mapper;

        public PaymentServices(IPaymentRepository paymentRepository, IPaymentStatusHistoryRepository statusHistoryRepository, IPaymentMethodRepository paymentMethodRepository)
        {
            _mapper = new PaymentConverter();
            _paymentRepository = paymentRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<PaymentDto> CreateAsync(PaymentDto payment)
        {
            Payment NewPayment = _mapper.Parse(payment);
            NewPayment.CreatedAt = DateTimeOffset.Now.DateTime;
            NewPayment.Status = Models.Constants.PaymentStatus.CREATED;
            NewPayment.UpdatedAt = NewPayment.CreatedAt;
            Payment CreatedPayment = await _paymentRepository.CreateAsync(NewPayment);

            PaymentStatusHistory paymentUpdate = new(CreatedPayment.Id, CreatedPayment.Status, null, NewPayment.CreatedAt);
            await _statusHistoryRepository.CreateHistoryInfoAsync(paymentUpdate);

            return _mapper.Parse(CreatedPayment);
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

        public async Task<PaymentDto> UpdateAsync(PaymentStatusHistory paymentStatus)
        {
            Payment PaymentFound = await _paymentRepository.FindByIdAsync(paymentStatus.PaymentId);
            if(PaymentFound == null) throw new ArgumentNullException(nameof(PaymentFound));

            paymentStatus.ChangedAt = DateTimeOffset.Now.DateTime;
            PaymentStatusHistory NewStatus = await _statusHistoryRepository.CreateHistoryInfoAsync(paymentStatus);

            PaymentFound.Status = paymentStatus.Status;
            PaymentFound.UpdatedAt = DateTimeOffset.Now.DateTime;

            Payment PaymentUpdated = await _paymentRepository.UpdateAsync(PaymentFound);

            return _mapper.Parse(PaymentUpdated);
        }

        public async Task<Models.Entities.PaymentMethod> FindDetailsByIdAsync(long id) {
            return await _paymentMethodRepository.FindByIdAsync(id);
        }

        public async Task<Models.Entities.PaymentMethod> FindDetailsByPaymentIdAsync(Guid PaymentId) {
            return await _paymentMethodRepository.FindByPaymentAsync(PaymentId);
        }

        public async Task<List<Models.Entities.PaymentMethod>> FindDetailsByPaymentTypeAsync(Models.Constants.PaymentType type) {
            return await _paymentMethodRepository.FindByPaymentTypeAsync(type);
        }

        public async Task<Models.Entities.PaymentMethod> CreatePaymentDetailsAsync(Models.Entities.PaymentMethod paymentMethodDetails) {
            return await _paymentMethodRepository.CreateAsync(paymentMethodDetails);
        }

        public async Task<Models.Entities.PaymentMethod> UpdatePaymentDetailsAsync(Models.Entities.PaymentMethod paymentMethodDetailsUpdate) {
            await _paymentMethodRepository.UpdateAsync(paymentMethodDetailsUpdate);
            return await _paymentMethodRepository.FindByIdAsync(paymentMethodDetailsUpdate.Id);
        }

        public async Task<Models.Entities.PaymentMethod> DeletePaymentDetailsAsync(long id) {
            return await _paymentMethodRepository.DeleteAsync(id);
        }

    }
}
