using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities
{
    public class Address
    {
        [Required]
        public string Street { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }

    }
}
