using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreatePaymentRequest
    {
        [JsonPropertyName("customerID")]
        public Guid CustomerID { get; set; }

        [Required]
        [MaxLength(100)]
        public string MerchantReference { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("originalPaymentAmount")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal OriginalPaymentAmount { get; set; }

        [JsonPropertyName("currency")]
        [RegularExpression("^[A-Z]{3}$")]
        public string Currency { get; set; } = "SEK";
    }
}
