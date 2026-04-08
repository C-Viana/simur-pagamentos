using Microsoft.Extensions.Logging;
using Moq;
using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.CustomerRepository;
using simur_backend.Services.Customers;

namespace simur_backend_tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<ILogger<CustomerServices>> _loggerMock;
    private readonly CustomerServices _service;
    private readonly CustomerConverter _mapper;

    private readonly string _customerDoc = "12345678901";
    private readonly Customer _TestCustomer;

    public CustomerServiceTests()
    {
        _repositoryMock = new();
        _loggerMock = new();
        _service = new(_repositoryMock.Object);
        _mapper = new();

        _TestCustomer = new Customer(
            Guid.NewGuid(),
            "João Silva",
            _customerDoc,
            "jsilva@testmail.com",
            "11977011510",
            DateOnly.Parse("1988-11-16"),
            new Address
            {
                PostalCode = "04110950",
                Country = "Brasil",
                Street = "Rua Cruzeiro do Sul",
                Number = 374,
                City = "São Paulo",
                State = "São Paulo"
            },
            "josilva09"
        );
    }

    [Fact]
    public async Task FindCustomerByDocumentAsync_WhenExists_ReturnsCustomerDto()
    {
        _repositoryMock.Setup(res => res.FindCustomerByDocumentAsync(_customerDoc)).ReturnsAsync(_TestCustomer);

        CustomerDto result = await _service.FindCustomerByDocumentAsync(_customerDoc);
        Assert.NotNull( result );
        Assert.Equal(_customerDoc, result.Document);
    }

    [Fact]
    public async Task FindCustomerByDocumentAsync_WhenNotExists_ReturnsNull()
    {
        Customer? nullCustomer = null;
        _repositoryMock.Setup(res => res.FindCustomerByDocumentAsync("00000000000")).ReturnsAsync(nullCustomer);
        CustomerDto result = await _service.FindCustomerByDocumentAsync("00000000000");
        Assert.Null(result);
    }

    [Fact]
    public async Task FindCustomerByIdAsync_WhenExists_ReturnsCustomerDto()
    {
        _repositoryMock.Setup(res => res.FindCustomerByIdAsync(_customerDoc)).ReturnsAsync(_TestCustomer);
        CustomerDto result = await _service.FindCustomerByIdAsync(_customerDoc);
        Assert.NotNull(result);
        Assert.Equal(_customerDoc, result.Document);
    }

    [Fact]
    public async Task FindCustomerByIdAsync_WhenNotExists_ReturnsNull()
    {
        Customer? nullCustomer = null;
        _repositoryMock.Setup(res => res.FindCustomerByIdAsync("00000000000")).ReturnsAsync(nullCustomer);
        CustomerDto result = await _service.FindCustomerByIdAsync("00000000000");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCustomerAsync_WhenSuccess_ReturnsCustomerDto()
    {
        Customer newCustomer = new(
            "Maria da Costa Ribeiro",
            _customerDoc,
            "mcribeiro@testmail.com",
            "11977011510",
            DateOnly.Parse("1988-11-16"),
            new Address
            {
                PostalCode = "04110950",
                Country = "Brasil",
                Street = "Rua Cruzeiro do Sul",
                Number = 374,
                City = "São Paulo",
                State = "São Paulo"
            },
            "mcribeiro005"
        );

        CustomerDto customerRequest = _mapper.Parse(newCustomer);
        _repositoryMock.Setup(res => res.CreateAsync(newCustomer));
        _repositoryMock.Setup(res => res.FindCustomerByDocumentAsync(newCustomer.Document)).ReturnsAsync(newCustomer);

        CustomerDto result = await _service.CreateCustomerAsync(customerRequest);
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Id.ToString()));
        Assert.Equal(_customerDoc, result.Document);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenDifferent_ReturnsUpdatedCustomerDto()
    {
        Customer? updatedCustomer = new(
            _TestCustomer.Id,
            _TestCustomer.FullName,
            _TestCustomer.Document,
            "jsilva1988@livemail.com",
            _TestCustomer.Phone,
            _TestCustomer.Birthdate,
            _TestCustomer.Address,
            _TestCustomer.ExternalBuyerId
        );
        _repositoryMock.Setup(res => res.UpdateCustomerAsync(updatedCustomer)).ReturnsAsync(updatedCustomer);
        CustomerDto result = await _service.UpdateCustomerAsync(_mapper.Parse(_TestCustomer), _mapper.Parse(updatedCustomer));
        Assert.NotEqual(updatedCustomer?.Email, _TestCustomer.Email);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenEqual_ReturnsSameCustomerDto()
    {
        Customer? updatedCustomer = new(
            _TestCustomer.Id,
            _TestCustomer.FullName,
            _TestCustomer.Document,
            _TestCustomer.Email,
            _TestCustomer.Phone,
            _TestCustomer.Birthdate,
            _TestCustomer.Address,
            _TestCustomer.ExternalBuyerId
        );
        _repositoryMock.Setup(res => res.UpdateCustomerAsync(updatedCustomer)).ReturnsAsync(updatedCustomer);
        CustomerDto result = await _service.UpdateCustomerAsync(_mapper.Parse(_TestCustomer), _mapper.Parse(updatedCustomer));
        Assert.Equal(updatedCustomer?.Email, _TestCustomer.Email);
    }

    [Fact]
    public async Task DeleteCustomerByDocumentAsync_WhenExists_ReturnsTrue()
    {
        _repositoryMock.Setup(res => res.DeleteCustomerAsync(_customerDoc)).ReturnsAsync(true);
        bool? result = await _service.DeleteCustomerByDocumentAsync(_customerDoc);
        Assert.Equal(true, result);
    }

    [Fact]
    public async Task DeleteCustomerByDocumentAsync_WhenNotExists_ReturnsFalse()
    {
        _repositoryMock.Setup(res => res.DeleteCustomerAsync("00000000000")).ReturnsAsync(false);
        bool? result = await _service.DeleteCustomerByDocumentAsync("00000000000");
        Assert.Equal(false, result);
    }
}
