using Paymentprocess.Api.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreatePaymentResponse
    {
        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("customerID")]
        public Guid CustomerID { get; set; }

        [JsonPropertyName("merchantReference")]
        public string MerchantRefrence { get; set; } = string.Empty;

        [JsonPropertyName("originalPaymentAmount")]
        public decimal OriginalPaymentAmount { get; set; }

        [JsonPropertyName("outstandingAmount")]
        public decimal OutstandingAmount { get; set; }


        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "SEK";


        [JsonPropertyName("paymentStatus")]
        public PaymentStatus PaymentStatus { get; set; }

        [JsonPropertyName("paymentDate")]
        public DateTime CreationDate { get; set; }

    }
}
