using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreateRepaymentRequest
    {
        [Required]
        [JsonPropertyName("repaymentAmount")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal RepaymentAmount { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; } = string.Empty;
    }
}
