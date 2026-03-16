using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.DTO.V1
{
    public class MerchantDto
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string TradeName { get; set; }

        [Required]
        [Length(14, 14)]
        public string Document { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PixKey { get; set; }

        public string MCC { get; set; } //Merchant Category Code: according ISO 18245

        public string BankAccountId { get; set; }

        public Address Address { get; set; }
    }
}
