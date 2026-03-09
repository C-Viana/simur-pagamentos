using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.Entities
{
    public class Customer
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [NotNull]
        public string FullName { get; set; }

        [Required]
        [NotNull]
        [Length(11, 14)]
        public string Document { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public DateOnly? Birthdate { get; set; }
        public Address? Address { get; set; }
        public string? ExternalBuyerId { get; set; }

        public Customer(Guid id, string fullName, string document, string email, string phone, DateOnly? birthdate, Address? address, string? externalBuyerId)
        {
            Id = id;
            FullName = fullName;
            Document = document;
            Email = email;
            Phone = phone;
            Birthdate = birthdate;
            Address = address;
            ExternalBuyerId = externalBuyerId;
        }

        public Customer(string fullName, string document, string email, string phone, DateOnly? birthdate, Address? address, string? externalBuyerId)
        {
            FullName = fullName;
            Document = document;
            Email = email;
            Phone = phone;
            Birthdate = birthdate;
            Address = address;
            ExternalBuyerId = externalBuyerId;
        }
    }
}
