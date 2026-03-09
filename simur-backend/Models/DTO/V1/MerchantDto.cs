using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.DTO.V1
{
    public class MerchantDto
    {
        [BsonId]
        [JsonPropertyName("id")]
        [MaybeNull]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Length(14, 14)]
        [JsonPropertyName("document")]
        public string Document { get; set; }

        [Required]
        [JsonPropertyName("trade_name")]
        public string TradeName { get; set; }

        [Required]
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("address")]
        public Address Address { get; set; }

        [JsonPropertyName("pix_key")]
        public string PixKey { get; set; }

        [JsonPropertyName("bank_account_id")]
        public string BankAccountId { get; set; }
    }
}
