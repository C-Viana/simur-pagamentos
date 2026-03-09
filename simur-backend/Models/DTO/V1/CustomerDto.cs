using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.DTO.V1
{
    public class CustomerDto
    {
        [BsonId]
        [JsonPropertyName("id")]
        [MaybeNull]
        public Guid Id { get; set; }

        [JsonPropertyName("full_name")]
        [Required]
        public string FullName { get; set; }

        [JsonPropertyName("document")]
        [Required]
        [Length(11, 14)]
        public string Document { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("birthdate")]
        public DateOnly? Birthdate { get; set; }

        [JsonPropertyName("address")]
        public Address? Address { get; set; }

        [JsonPropertyName("buyer_id")]
        public string? ExternalBuyerId { get; set; }
    }
}
