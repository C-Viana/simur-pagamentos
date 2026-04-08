using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using SharpCompress.Common;
using simur_backend.Mappers;
using simur_backend.Messaging;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.PaymentRepository;
using simur_backend.Services;
using simur_backend.Services.Payments;
using simur_backend_tests.Utils;

namespace simur_backend_tests.UnitTests.Services
{
    public class PaymentServicesTests
    {
        private readonly Mock<IPaymentRepository> _paymentMock;
        private readonly Mock<IPaymentMethodRepository> _methodMock;
        private readonly Mock<IPaymentStatusHistoryRepository> _statusMock;
        private readonly Mock<ILogger<PaymentServices>> _loggerMock;
        private Mock<IMongoClient> _mongoMock;
        private readonly Mock<IClientSessionHandle> _sessionMock;
        private readonly Mock<IMerchantServices> _merchantMock;
        private readonly Mock<IMessageBusService> _brokerMock;
        private readonly PaymentServices _service;
        private readonly PaymentConverter _mapper;

        public PaymentServicesTests()
        {
            _sessionMock = new();
            _mongoMock = new();
            _paymentMock = new();
            _methodMock = new();
            _statusMock = new();
            _loggerMock = new();
            _merchantMock = new();
            _brokerMock = new();
            _service = new(_paymentMock.Object, _statusMock.Object, _methodMock.Object, _loggerMock.Object, _mongoMock.Object, _merchantMock.Object, _brokerMock.Object);
            _mapper = new PaymentConverter();

            _sessionMock.Setup(x => x.StartTransaction(It.IsAny<TransactionOptions>()));
            _sessionMock.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _sessionMock.Setup(x => x.AbortTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mongoMock.Setup(x => x.StartSessionAsync()).ReturnsAsync(_sessionMock.Object);
        }

        [Fact]
        public async Task DeletePayment_WhenExists_ReturnDto()
        {
            Payment payment = PaymentTestsUtils.getPayment();
            PaymentMethod method = new(payment.Id, PaymentType.BOLETO, PaymentTestsUtils.getBoletoDetails());

            _paymentMock.Setup(repo => repo.DeleteAsync(payment.Id)).ReturnsAsync(payment);
            _statusMock.Setup(repo => repo.DeleteAllPaymentHistoryAsync(payment.Id)).ReturnsAsync(1);
            _methodMock.Setup(repo => repo.DeleteByPaymentIdAsync(payment.Id)).ReturnsAsync(method);

            PaymentDto result = await _service.DeleteAsync(payment.Id);

            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.Id);
            _paymentMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
            _statusMock.Verify(repo => repo.DeleteAllPaymentHistoryAsync(It.IsAny<Guid>()), Times.Once);
            _methodMock.Verify(repo => repo.DeleteByPaymentIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeletePayment_WhenNotExists_JumpsToReturn()
        {
            Guid id = Guid.NewGuid();
            _paymentMock.Setup(repo => repo.DeleteAsync(id)).ReturnsAsync((Payment)null);

            PaymentDto result = await _service.DeleteAsync(id);

            Assert.Null(result);
            _paymentMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
            _statusMock.Verify(repo => repo.DeleteAllPaymentHistoryAsync(It.IsAny<Guid>()), Times.Never);
            _methodMock.Verify(repo => repo.DeleteByPaymentIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task FindByCreatedAtAsync_WhenExists_ReturnListDto()
        {
            DateOnly currentDate = new();
            int totalPayments = 2;
            List<PaymentDto> paymentsDto = [];
            List<Payment> payments = [];
            for (int i = 0; i < totalPayments; i++)
            {
                paymentsDto.Add(PaymentTestsUtils.getPaymentDto());
            }
            payments.AddRange(_mapper.ParseList(paymentsDto));

            _paymentMock.Setup(repo => repo.FindByCreatedAtAsync(currentDate)).ReturnsAsync(payments);

            List<PaymentDto> result = await _service.FindByCreatedAtAsync(currentDate);

            Assert.Equal(totalPayments, result.Count);
            Assert.Equal(paymentsDto.Count, result.Count);
            _paymentMock.Verify(repo => repo.FindByCreatedAtAsync(It.IsAny<DateOnly>()), Times.Once);
        }

        [Fact]
        public async Task FindByCustomerDocAsync_WhenExists_ReturnListDto()
        {
            string customerDoc = "26499704102";
            int totalPayments = 2;
            List<PaymentDto> paymentsDto = [];
            List<Payment> payments = [];
            for (int i = 0; i < totalPayments; i++)
            {
                paymentsDto.Add(PaymentTestsUtils.getPaymentDto());
            }
            payments.AddRange(_mapper.ParseList(paymentsDto));

            _paymentMock.Setup(repo => repo.FindByCustomerDocAsync(customerDoc)).ReturnsAsync(payments);

            List<PaymentDto> result = await _service.FindByCustomerDocAsync(customerDoc);

            Assert.Equal(totalPayments, result.Count);
            Assert.Equal(paymentsDto.Count, result.Count);
            _paymentMock.Verify(repo => repo.FindByCustomerDocAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task FindByMerchantDocAsync_WhenExists_ReturnListDto()
        {
            string merchantDoc = "53178402000147";
            int totalPayments = 2;
            List<PaymentDto> paymentsDto = [];
            List<Payment> payments = [];
            for (int i = 0; i < totalPayments; i++)
            {
                paymentsDto.Add(PaymentTestsUtils.getPaymentDto());
            }
            payments.AddRange(_mapper.ParseList(paymentsDto));

            _paymentMock.Setup(repo => repo.FindByMerchantDocAsync(merchantDoc)).ReturnsAsync(payments);

            List<PaymentDto> result = await _service.FindByMerchantDocAsync(merchantDoc);

            Assert.Equal(totalPayments, result.Count);
            Assert.Equal(paymentsDto.Count, result.Count);
            _paymentMock.Verify(repo => repo.FindByMerchantDocAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task FindByExternalOrderIdAsync_WhenExists_ReturnDto()
        {
            string externalOrderId = "PF-20260403165626-9853";
            PaymentDto paymentsDto = PaymentTestsUtils.getPaymentDto();
            Payment payment = _mapper.Parse(paymentsDto);
            _paymentMock.Setup(repo => repo.FindByExternalOrderIdAsync(externalOrderId)).ReturnsAsync(payment);

            PaymentDto result = await _service.FindByExternalOrderIdAsync(externalOrderId);

            Assert.NotNull(result);
            Assert.Equal(paymentsDto.ExternalOrderId, result.ExternalOrderId);
            _paymentMock.Verify(repo => repo.FindByExternalOrderIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task FindByIdAsync_WhenExists_ReturnDto()
        {
            Payment payment = PaymentTestsUtils.getPayment();
            _paymentMock.Setup(repo => repo.FindByIdAsync(payment.Id)).ReturnsAsync(payment);

            PaymentDto result = await _service.FindByIdAsync(payment.Id);

            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.Id);
            _paymentMock.Verify(repo => repo.FindByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
