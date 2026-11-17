using Paymentprocess.Api.Models;

namespace Paymentprocess.Api.DTOs
{
    public class IdempotencyRecord
    {
        public Guid ID { get; set; }

        public string IdempotencyKey { get; set; } = string.Empty;

        public string RequestHash { get; set; } = string.Empty;

        public Guid? RepaymentID { get; set; }

        public DateTime CreationDate { get; set; }

        // Navigation property to the Repayment entity
        public Repayment? Repayment { get; set; }
    }
}
