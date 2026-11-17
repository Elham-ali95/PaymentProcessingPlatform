using Paymentprocess.Api.Enums;

namespace Paymentprocess.Api.Models
{
    public class Repayment
    {
        public Guid ID { get; set; }
        public Guid PaymentIdentifier { get; set; }
        public decimal RepaymentAmount { get; set; }
        public string ExternalRefrence { get; set; } = string.Empty;    
        public DateTime CreationDate { get; set; }

        public RepaymentStatus RepaymentStatus { get; set; }

        // Navigation property to the Payments entity
        public Payments Payment { get; set; } = null!;
    }
}
