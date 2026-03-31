using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.Entities
{
    public class Merchant
    {
        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Document { get; set; }
        public string TradeName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        public string PixKey { get; set; }
        public string MCC { get; set; } //Merchant Category Code: according ISO 18245
        public string BankAccountId { get; set; }

        public Merchant(string companyName, string tradeName, string document, string email, string phoneNumber, Address address, string pixKey, string mcc, string bankAccountId)
        {
            Document = document;
            TradeName = tradeName;
            CompanyName = companyName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            PixKey = pixKey;
            MCC = mcc;
            BankAccountId = bankAccountId;
        }

        public Merchant(Guid id, string companyName, string tradeName, string document, string email, string phoneNumber, Address address, string pixKey, string mcc, string bankAccountId)
        {
            Id = id;
            Document = document;
            TradeName = tradeName;
            CompanyName = companyName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            PixKey = pixKey;
            MCC = mcc;
            BankAccountId = bankAccountId;
        }
    }
}
