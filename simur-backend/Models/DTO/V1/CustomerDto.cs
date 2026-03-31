using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Hypermedia;
using simur_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.DTO.V1
{
    public class CustomerDto : ISupportHypermedia
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        [NotNull]
        public string FullName { get; set; }

        [Required]
        [NotNull]
        [Length(11, 14)]
        public string Document { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression("^\\d{10,11}$")]
        public string Phone { get; set; }

        [RegularExpression("^\\d{4}-\\d{2}-\\d{2}$")]
        public DateOnly Birthdate { get; set; }

        public Address Address { get; set; }

        public string ExternalBuyerId { get; set; }

        public List<HypermediaLinks> Links { get; set; } = [];
    }
}
