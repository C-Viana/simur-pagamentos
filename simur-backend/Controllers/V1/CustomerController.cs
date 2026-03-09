using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using simur_backend.Models.DTO.V1;
using simur_backend.Services;
using simur_backend.Services.Customers;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace simur_backend.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet("document/{document}", Name = "FindCustomerByDocument")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FindCustomerByDocument(string document)
    {
        _logger.LogInformation("Fetching for customer document {document}", document);
        CustomerDto FoundEntity = await _customerService.FindCustomerByDocumentAsync(document);
        if(FoundEntity == null)
        {
            _logger.LogInformation("No customer found with document {document}", document);
            return NotFound($"No customer found with document {document}");
        }
        return Ok(FoundEntity);
    }

    [HttpGet("{id}", Name = "FindCustomerById")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FindCustomerById(string id)
    {
        _logger.LogInformation("No customer found with document {id}", id);
        CustomerDto FoundEntity = await _customerService.FindCustomerByIdAsync(id);
        if (FoundEntity == null)
        {
            _logger.LogInformation("No customer found with document {id}", id);
            return NotFound($"No customer found with id {id}");
        }
        return Ok(FoundEntity);
    }

    [HttpPost(Name = "CreateCustomer")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customer)
    {
        _logger.LogInformation("Creating a new customer with document {document}", customer.Document);
        if(_customerService.FindCustomerByDocumentAsync(customer.Document).Result != null)
        {
            _logger.LogInformation("Customer with document {document} already exists", customer.Document);
            return BadRequest($"Customer with document {customer.Document} already exists");
        }
        CustomerDto entity = await _customerService.CreateCustomerAsync(customer);
        _logger.LogInformation("Customer with document {document} successfully created", entity.Document);
        return Created(nameof(CreateCustomer), entity);
    }

    [HttpPut(Name = "UpdateCustomer")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto updateCustomer)
    {
        if (string.IsNullOrWhiteSpace(updateCustomer.Id.ToString()))
            return BadRequest("Customer ID must be informed to update a document");

        CustomerDto CurrentCustomer = await _customerService.FindCustomerByIdAsync(updateCustomer.Id.ToString());
        if (CurrentCustomer == null) return BadRequest("Customer not found");

        _logger.LogInformation("Update required for customer with document {document}", updateCustomer.Document);
        CustomerDto UpdatedEntity = await _customerService.UpdateCustomerAsync(CurrentCustomer, updateCustomer);
        if(UpdatedEntity == null)
        {
            _logger.LogWarning("Customer with document {document} could not be updated due data unconformity", updateCustomer.Document);
            return BadRequest("Fields previously saved cannot be updated to null or empty values.");
        }
        _logger.LogInformation("Customer with document {document} successfully updated", UpdatedEntity.Document);
        return Ok(UpdatedEntity);
    }

    [HttpDelete("document/{document}", Name = "DeleteCustomerByDocument")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCustomerByDocument(string document)
    {
        _logger.LogInformation("Deletion required for customer with document {document}", document);
        bool isDeleted = await _customerService.DeleteCustomerByDocumentAsync(document);
        if (isDeleted) _logger.LogInformation("Customer with document {document} successfully deleted", document);
        else _logger.LogInformation("Deletion was not executed as customer with document {document} didn't exist", document);
        return NoContent();
    }
}
