using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreateCustomerRequest
    {
        [Required]
        [StringLength(100)]
        [JsonPropertyName("firstname")]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [JsonPropertyName("lastname")]
        public string Lastname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(265)]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}
