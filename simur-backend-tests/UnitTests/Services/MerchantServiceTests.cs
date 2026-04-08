using Microsoft.Extensions.Logging;
using Moq;
using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.MerchantRepository;
using simur_backend.Services.Merchants;

namespace simur_backend_tests;

public class MerchantServiceTests
{
    private readonly Mock<IMerchantRepository> _repositoryMock;
    private readonly MerchantServices _service;
    private readonly MerchantConverter _mapper;

    private readonly string _merchantDoc = "12345670008910";
    private readonly Merchant _TestMerchant;

    public MerchantServiceTests()
    {
        _repositoryMock = new();
        _service = new(_repositoryMock.Object);
        _mapper = new();

        _TestMerchant = new Merchant(
            Guid.NewGuid(),
            "Quality Assurance Ltda",
            "QA Ltda",
            _merchantDoc,
            "qacompany@testmail.com",
            "11977011510",
            new Address
            {
                PostalCode = "04110950",
                Country = "Brasil",
                Street = "Rua Cruzeiro do Sul",
                Number = 374,
                City = "São Paulo",
                State = "São Paulo"
            },
            _merchantDoc,
            "1054",
            "100005468"
        );
    }

    [Fact]
    public async Task FindMerchantByDocumentAsync_WhenExists_ReturnsMerchantDto()
    {
        _repositoryMock.Setup(res => res.FindMerchantByDocumentAsync(_merchantDoc)).ReturnsAsync(_TestMerchant);

        MerchantDto result = await _service.FindMerchantByDocumentAsync(_merchantDoc);
        Assert.NotNull( result );
        Assert.Equal(_merchantDoc, result.Document);
    }

    [Fact]
    public async Task FindMerchantByDocumentAsync_WhenNotExists_ReturnsNull()
    {
        Merchant? nullMerchant = null;
        _repositoryMock.Setup(res => res.FindMerchantByDocumentAsync("00000000000")).ReturnsAsync(nullMerchant);
        MerchantDto result = await _service.FindMerchantByDocumentAsync("00000000000");
        Assert.Null(result);
    }

    [Fact]
    public async Task FindMerchantByIdAsync_WhenExists_ReturnsMerchantDto()
    {
        _repositoryMock.Setup(res => res.FindMerchantByIdAsync(_merchantDoc)).ReturnsAsync(_TestMerchant);
        MerchantDto result = await _service.FindMerchantByIdAsync(_merchantDoc);
        Assert.NotNull(result);
        Assert.Equal(_merchantDoc, result.Document);
    }

    [Fact]
    public async Task FindMerchantByIdAsync_WhenNotExists_ReturnsNull()
    {
        Merchant? nullMerchant = null;
        _repositoryMock.Setup(res => res.FindMerchantByIdAsync("00000000000")).ReturnsAsync(nullMerchant);
        MerchantDto result = await _service.FindMerchantByIdAsync("00000000000");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateMerchantAsync_WhenSuccess_ReturnsMerchantDto()
    {
        Merchant newMerchant = new(
            "Great Test Corporation",
            "GT Corp",
            _merchantDoc,
            "administration@gtcorp.com",
            "11977011510",
            new Address
            {
                PostalCode = "04110950",
                Country = "Brasil",
                Street = "Rua Cruzeiro do Sul",
                Number = 374,
                City = "São Paulo",
                State = "São Paulo"
            },
            _merchantDoc,
            "1054",
            "100005468"
        );

        MerchantDto merchantRequest = _mapper.Parse(newMerchant);
        _repositoryMock.Setup(res => res.CreateAsync(newMerchant));
        _repositoryMock.Setup(res => res.FindMerchantByDocumentAsync(newMerchant.Document)).ReturnsAsync(newMerchant);

        MerchantDto result = await _service.CreateMerchantAsync(merchantRequest);
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Id.ToString()));
        Assert.Equal(_merchantDoc, result.Document);
    }

    [Fact]
    public async Task UpdateMerchantAsync_WhenDifferent_ReturnsUpdatedMerchantDto()
    {
        Merchant? updatedMerchant = new(
            _TestMerchant.Id,
            _TestMerchant.CompanyName,
            _TestMerchant.TradeName,
            _TestMerchant.Document,
            "contact@qacompany.com",
            _TestMerchant.PhoneNumber,
            _TestMerchant.Address,
            _TestMerchant.PixKey,
            _TestMerchant.MCC,
            _TestMerchant.BankAccountId
        );
        _repositoryMock.Setup(res => res.UpdateMerchantAsync(updatedMerchant)).ReturnsAsync(updatedMerchant);
        MerchantDto result = await _service.UpdateMerchantAsync(_mapper.Parse(_TestMerchant), _mapper.Parse(updatedMerchant));
        Assert.NotEqual(updatedMerchant?.Email, _TestMerchant.Email);
    }

    [Fact]
    public async Task UpdateMerchantAsync_WhenEqual_ReturnsSameMerchantDto()
    {
        Merchant? updatedMerchant = new(
            _TestMerchant.Id,
            _TestMerchant.CompanyName,
            _TestMerchant.TradeName,
            _TestMerchant.Document,
            _TestMerchant.Email,
            _TestMerchant.PhoneNumber,
            _TestMerchant.Address,
            _TestMerchant.PixKey,
            _TestMerchant.MCC,
            _TestMerchant.BankAccountId
        );
        _repositoryMock.Setup(res => res.UpdateMerchantAsync(updatedMerchant)).ReturnsAsync(updatedMerchant);
        MerchantDto result = await _service.UpdateMerchantAsync(_mapper.Parse(_TestMerchant), _mapper.Parse(updatedMerchant));
        Assert.Equal(updatedMerchant?.Email, _TestMerchant.Email);
    }

    [Fact]
    public async Task DeleteMerchantByDocumentAsync_WhenExists_ReturnsTrue()
    {
        _repositoryMock.Setup(res => res.DeleteMerchantAsync(_merchantDoc)).ReturnsAsync(true);
        bool? result = await _service.DeleteMerchantByDocumentAsync(_merchantDoc);
        Assert.Equal(true, result);
    }

    [Fact]
    public async Task DeleteMerchantByDocumentAsync_WhenNotExists_ReturnsFalse()
    {
        _repositoryMock.Setup(res => res.DeleteMerchantAsync("00000000000000")).ReturnsAsync(false);
        bool? result = await _service.DeleteMerchantByDocumentAsync("00000000000000");
        Assert.Equal(false, result);
    }
}
