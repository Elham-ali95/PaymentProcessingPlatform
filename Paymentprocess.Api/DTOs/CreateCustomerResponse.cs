using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreateCustomerResponse
    {
        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; } = string.Empty;

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }
    }
}
