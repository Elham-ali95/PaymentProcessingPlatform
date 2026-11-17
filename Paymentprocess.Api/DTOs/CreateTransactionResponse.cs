using Paymentprocess.Api.Enums;
using System.Text.Json.Serialization;

namespace Paymentprocess.Api.DTOs
{
    public class CreateTransactionResponse
    {
        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("paymentId")]
        public Guid PaymentID { get; set; }

        [JsonPropertyName("transactionType")]
        public TransactionType transactionType { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }
    }
}
