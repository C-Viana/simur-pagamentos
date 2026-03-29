using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simur_backend.Models.DTO.V1;
using simur_backend.Services;

namespace simur_backend.Controllers.V1
{
    [ApiController]
    [Authorize("Bearer")]
    [Route("api/v1/[controller]")]
    public class MerchantController : Controller
    {
        private readonly IMerchantService _merchantService;
        private readonly ILogger<MerchantController> _logger;

        public MerchantController(IMerchantService merchantService, ILogger<MerchantController> logger)
        {
            _merchantService = merchantService;
            _logger = logger;
        }

        [HttpGet("document/{document}", Name = "FindMerchantByDocument")]
        [ProducesResponseType(typeof(MerchantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindMerchantByDocument(string document)
        {
            _logger.LogInformation("Fetching for merchant document {document}", document);
            MerchantDto FoundEntity = await _merchantService.FindMerchantByDocumentAsync(document);
            if (FoundEntity == null)
            {
                _logger.LogInformation("No merchant found with document {document}", document);
                return NotFound($"No merchant found with document {document}");
            }
            
            return Ok(FoundEntity);
        }

        [HttpGet("{id}", Name = "FindMerchantById")]
        [ProducesResponseType(typeof(MerchantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindMerchantById(string id)
        {
            _logger.LogInformation("No merchant found with document {id}", id);
            MerchantDto FoundEntity = await _merchantService.FindMerchantByIdAsync(id);
            if (FoundEntity == null)
            {
                _logger.LogInformation("No merchant found with document {id}", id);
                return NotFound($"No merchant found with id {id}");
            }
            return Ok(FoundEntity);
        }

        [HttpPost(Name = "CreateMerchant")]
        [ProducesResponseType(typeof(MerchantDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMerchant([FromBody] MerchantDto merchant)
        {
            _logger.LogInformation("Creating a new merchant with document {document}", merchant.Document);
            if (await _merchantService.FindMerchantByDocumentAsync(merchant.Document) != null)
            {
                _logger.LogInformation("Merchant with document {document} already exists", merchant.Document);
                return BadRequest($"Merchant with document {merchant.Document} already exists");
            }
            MerchantDto entity = await _merchantService.CreateMerchantAsync(merchant);
            _logger.LogInformation("Merchant with document {document} successfully created", entity.Document);
            return Created(nameof(CreateMerchant), entity);
        }

        [HttpPut(Name = "UpdateMerchant")]
        [ProducesResponseType(typeof(MerchantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMerchant([FromBody] MerchantDto merchant)
        {
            if (string.IsNullOrWhiteSpace(merchant.Id.ToString()))
                return BadRequest("Merchant ID must be informed to update a document");

            MerchantDto CurrentMerchant = await _merchantService.FindMerchantByIdAsync(merchant.Id.ToString());
            if (CurrentMerchant == null) return BadRequest("Merchant not Found");

            _logger.LogInformation("Update required for merchant with document {document}", merchant.Document);
            MerchantDto UpdatedEntity = await _merchantService.UpdateMerchantAsync(CurrentMerchant, merchant);
            if (UpdatedEntity == null)
            {
                _logger.LogWarning("Merchant with document {document} could not be updated due data unconformity", merchant.Document);
                return BadRequest("Fields previously saved cannot be updated to null or empty values.");
            }
            _logger.LogInformation("Merchant with document {document} successfully updated", UpdatedEntity.Document);
            return Ok(UpdatedEntity);
        }

        [HttpDelete("document/{document}", Name = "DeleteMerchantByDocument")]
        [ProducesResponseType(typeof(MerchantDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMerchantByDocument(string document)
        {
            _logger.LogInformation("Deletion required for merchant with document {document}", document);
            bool isDeleted = await _merchantService.DeleteMerchantByDocumentAsync(document);
            if (isDeleted) _logger.LogInformation("Merchant with document {document} successfully deleted", document);
            else _logger.LogInformation("Deletion was not executed as merchant with document {document} didn't exist", document);
            return NoContent();
        }
    }
}
