using Paymentprocess.Api.Enums;

namespace Paymentprocess.Api.Models
{
    public class Transactions
    {
        public Guid ID { get; set; }
        public Guid PaymentID { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        // Navigation property to the Payments entity
        public Payments Payment { get; set; } = null!;
    }
}
