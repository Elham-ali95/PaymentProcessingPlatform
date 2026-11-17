using Paymentprocess.Api.Enums;
using System.Transactions;

namespace Paymentprocess.Api.Models
{
    public class Payments
    {
        public  Guid ID { get; set; }

        public Guid CustomerID { get; set; }

        public string  MerchantReference { get; set; } = string.Empty;
        public decimal  OriginalPaymentAmount { get; set; }

        public decimal  OutstandingAmount { get; set; }
        public string   Currency { get; set; } = string.Empty;

        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreationDate { get; set; }


        // Navigation property to the Customers entity
        public Customers Customer { get; set; }

        public ICollection<Repayment> Repayments { get; set; } = new List<Repayment>();

        public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>();
    }
}
