using Paymentprocess.Api.Enums;
using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreateRepaymentResponse
    {
        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("paymentIdentifier")]
        public Guid PaymentIdentifier { get; set; }

        [JsonPropertyName("repaymentAmount")]
        public decimal RepaymentAmount { get; set; }

        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; } = string.Empty;

        [JsonPropertyName("paymentStatus")]
        public PaymentStatus PaymentStatus { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("repaymentStatus")]
        public RepaymentStatus RepaymentStatus { get; set; }
    }
}
