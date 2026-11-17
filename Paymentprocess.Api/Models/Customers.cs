namespace Paymentprocess.Api.Models
{
    public class Customers
    {
        public Guid ID { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        // Navigation property
        public ICollection<Payments> Payments { get; set; } = new List<Payments>();
    }
}
