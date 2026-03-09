using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.DTO.V1
{
    public class CustomerDto
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Length(11, 14)]
        public string Document { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateOnly Birthdate { get; set; }

        public Address Address { get; set; }

        public string ExternalBuyerId { get; set; }
    }
}
