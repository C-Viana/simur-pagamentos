using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities
{
    public class Address
    {
        [JsonPropertyName("street")]
        [Required]
        public string Street { get; set; }

        [JsonPropertyName("number")]
        [Required]
        public int Number { get; set; }

        [JsonPropertyName("city")]
        [Required]
        public string City { get; set; }

        [JsonPropertyName("state")]
        [Required]
        public string State { get; set; }

        [JsonPropertyName("postal_code")]
        [Required]
        public string PostalCode { get; set; }

        [JsonPropertyName("country")]
        [Required]
        public string Country { get; set; }

    }
}
