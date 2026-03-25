using Microsoft.AspNetCore.Mvc;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;

namespace simur_backend.Controllers.Gateway
{
    [Route("/api/v1/card/security")]
    [ApiController]
    public class PaymentCardTokenizerController : ControllerBase
    {
        private PaymentBrand DetectBrand(string cardNumber)
        {
            //PaymentBrand[] brands = Enum.GetValues<PaymentBrand>();
            //int totalBrands = brands.Length - 1;
            //return brands[new Random().Next(totalBrands)];

            if (cardNumber.StartsWith("4011") || cardNumber.StartsWith("4312") || cardNumber.StartsWith("4389") || cardNumber.StartsWith("6063") || cardNumber.StartsWith("6082"))
                return PaymentBrand.ELO;
            else if (cardNumber.StartsWith("51") || cardNumber.StartsWith("55") || cardNumber.StartsWith("2221") || cardNumber.StartsWith("2720"))
                return PaymentBrand.MASTERCARD;
            else if (cardNumber.StartsWith("30") || cardNumber.StartsWith("36") || cardNumber.StartsWith("38") || cardNumber.StartsWith("39"))
                return PaymentBrand.DINNERSCLUB;
            else if (cardNumber.StartsWith("6011") || cardNumber.StartsWith("65") || cardNumber.Contains("644-649"))
                return PaymentBrand.DISCOVER;
            else if (cardNumber.StartsWith("34") || cardNumber.StartsWith("37"))
                return PaymentBrand.AMEX;
            else if (cardNumber.StartsWith("6062"))
                return PaymentBrand.HIPERCARD;
            else if (cardNumber.StartsWith("4"))
                return PaymentBrand.VISA;
            else throw new ArgumentException("Invalid card number or brand not supported");
        }

        [HttpPost(Name = "Tokenize")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Tokenize([FromBody] PaymentCardRequestDto card)
        {
            if (card == null) return BadRequest("Payment card is null");
            var token = $"tok_simur_{Guid.NewGuid().ToString("N")[..16]}";
            var lastFour = card.CardNumber[^4..];
            var brand = DetectBrand(card.CardNumber);
            return Ok(new PaymentCardResponseDto
            {
                Token = token,
                LastFourDigits = lastFour,
                Brand = Enum.GetName<PaymentBrand>(brand)
            });
        }
    }
}
