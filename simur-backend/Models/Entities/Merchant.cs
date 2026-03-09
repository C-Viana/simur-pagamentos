using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.Entities
{
    public class Merchant
    {
        public Merchant(string companyName, string tradeName, string document, string email, string phoneNumber, Address address, string pixKey, string bankAccountId)
        {
            Document = document;
            TradeName = tradeName;
            CompanyName = companyName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            PixKey = pixKey;
            BankAccountId = bankAccountId;
        }

        public Merchant(Guid id, string companyName, string tradeName, string document, string email, string phoneNumber, Address address, string pixKey, string bankAccountId)
        {
            Id = id;
            Document = document;
            TradeName = tradeName;
            CompanyName = companyName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            PixKey = pixKey;
            BankAccountId = bankAccountId;
        }

        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [NotNull]
        public string Document { get; set; }

        [Required]
        [NotNull]
        public string TradeName { get; set; }

        [Required]
        [NotNull]
        public string CompanyName { get; set; }

        [Required]
        [NotNull]
        public string Email { get; set; }

        [Required]
        [NotNull]
        public string PhoneNumber { get; set; }

        [Required]
        [NotNull]
        public Address Address { get; set; }

        [Required]
        [NotNull]
        public string PixKey { get; set; }

        [Required]
        [NotNull]
        public string BankAccountId { get; set; }
    }
}
