using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Hypermedia;
using simur_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.DTO.V1
{
    public class MerchantDto : ISupportHypermedia
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        [NotNull]
        [Length(14, 14)]
        public string Document { get; set; }

        [Required]
        [NotNull]
        public string TradeName { get; set; }

        [Required]
        [NotNull]
        public string CompanyName { get; set; }

        [Required]
        [NotNull]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [NotNull]
        [RegularExpression("^\\d{10,11}$")]
        public string PhoneNumber { get; set; }

        public Address Address { get; set; }

        [Required]
        [NotNull]
        public string PixKey { get; set; }

        [Required]
        [NotNull]
        public string MCC { get; set; } //Merchant Category Code: according ISO 18245

        [Required]
        [NotNull]
        public string BankAccountId { get; set; }

        public List<HypermediaLinks> Links { get; set; } = [];
    }
}
