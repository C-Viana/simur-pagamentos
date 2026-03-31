using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simur_backend.Exceptions.CustomExceptions;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Services.Payments;

namespace simur_backend.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentServices _service;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentServices service, ILogger<PaymentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost(Name = "CreatePayment")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto payment)
        {
            _logger.LogInformation("Payment of type {type} requested", payment.PaymentDetails.PaymentType.ToString());
            _logger.LogInformation("Payment requested from {customer} to {seller} from order {order}", payment.PayerDocument, payment.SellerDocument, payment.ExternalOrderId);
            if (payment.PaymentDetails == null) throw new RequestInformationMissingException("Payment details are either missing or empty. Check the information and try again");

            PaymentDto savedPayment = await _service.CreateCompletePaymentAsync(payment, HttpContext);
            return CreatedAtAction(null, new { savedPayment.Id }, savedPayment);
        }

        [HttpGet("{paymentId}", Name = "FindPaymentById")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindPaymentById([FromRoute] string paymentId)
        {
            _logger.LogInformation("Fetching payment by internal ID {id}", paymentId);
            PaymentDto foundPayment = await _service.FindByIdAsync(Guid.Parse(paymentId));
            if(foundPayment == null) return NotFound($"No payment found for ID {paymentId}");
            PaymentMethod foundMethod = await _service.FindDetailsByPaymentIdAsync(Guid.Parse(paymentId));
            foundPayment.PaymentDetails = foundMethod.PaymentDetails;

            return Ok(foundPayment);
        }

        [HttpGet("date/{paymentDate}", Name = "FindByPaymentByDate")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindByPaymentByDate(DateOnly paymentDate)
        {
            _logger.LogInformation("Fetching payments created at {paymentDate}", paymentDate.ToString());

            List<PaymentDto> foundPayments = await _service.FindByCreatedAtAsync(paymentDate);

            if (foundPayments.Count < 1)
            {
                _logger.LogInformation("No payments found at {paymentDate}", paymentDate.ToString());
                return Ok($"No payments were created at {paymentDate.ToString()}");
            }
            foreach (var item in foundPayments)
            {
                PaymentMethod detail = await _service.FindDetailsByPaymentIdAsync(item.Id);
                item.PaymentDetails = detail.PaymentDetails;
            }
            _logger.LogInformation("A total of {count} payments found created at {paymentDate}", foundPayments.Count, paymentDate.ToString());
            return Ok(foundPayments);
        }

        [HttpGet("customer/{customerDoc}", Name = "FindPaymentByCustomerDocument")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindPaymentByCustomerDocument(string customerDoc)
        {
            _logger.LogInformation("Fetching payments created at {id}", customerDoc);
            List<PaymentDto> foundPayments = await _service.FindByCustomerDocAsync(customerDoc);

            if (foundPayments.Count < 1) return NotFound($"No payments found for customer {customerDoc}");
            foreach (var item in foundPayments)
            {
                PaymentMethod detail = await _service.FindDetailsByPaymentIdAsync(item.Id);
                item.PaymentDetails = detail.PaymentDetails;
            }

            return Ok(foundPayments);
        }

        [HttpGet("merchant/{merchantDoc}", Name = "FindPaymentByMerchantDoc")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindPaymentByMerchantDoc(string merchantDoc)
        {
            _logger.LogInformation("Fetching payments created at {id}", merchantDoc);
            List<PaymentDto> foundPayments = await _service.FindByMerchantDocAsync(merchantDoc);

            if (foundPayments.Count < 1) return NotFound($"No payments found for customer {merchantDoc}");
            foreach (var item in foundPayments)
            {
                PaymentMethod detail = await _service.FindDetailsByPaymentIdAsync(item.Id);
                item.PaymentDetails = detail.PaymentDetails;
            }

            return Ok(foundPayments);
        }

        [HttpGet("order/{externalOrderId}", Name = "FindPaymentByExternalOrderId")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindPaymentByExternalOrderId(string externalOrderId)
        {
            _logger.LogInformation("Fetching payment by order ID {id}", externalOrderId);
            PaymentDto foundPayment = await _service.FindByExternalOrderIdAsync(externalOrderId);
            if (foundPayment == null) return NotFound($"No payment found for order ID {externalOrderId}");
            PaymentMethod foundMethod = await _service.FindDetailsByPaymentIdAsync(foundPayment.Id);
            foundPayment.PaymentDetails = foundMethod.PaymentDetails;
            return Ok(foundPayment);
        }

        [HttpPut(Name = "UpdatePaymentStatus")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentStatusHistory paymentStatus)
        {
            _logger.LogInformation("Updating payment with ID {id}", paymentStatus.PaymentId);
            await _service.UpdatePaymentStatusAsync(paymentStatus);

            PaymentDto updatedPayment = await _service.FindByIdAsync(paymentStatus.PaymentId);
            if (updatedPayment == null) return BadRequest($"Update not executed. No payment found with ID {paymentStatus.PaymentId}");

            PaymentMethod foundMethod = await _service.FindDetailsByPaymentIdAsync(paymentStatus.PaymentId);
            updatedPayment.PaymentDetails = foundMethod.PaymentDetails;
            return Ok(updatedPayment);
        }

        [HttpDelete("{paymentId}", Name = "DeletePaymentById")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePaymentById([FromRoute] string paymentId)
        {
            _logger.LogInformation("Deleting payment with ID {id}", paymentId);
            PaymentDto updatedPayment = await _service.DeleteAsync(Guid.Parse(paymentId));
            if (updatedPayment == null) _logger.LogInformation("Deletion not executed. No payment found with ID {paymentId}", paymentId);
            else _logger.LogInformation("Payment with ID {paymentId} successfully deleted", paymentId);
            return NoContent();
        }
    }
}
