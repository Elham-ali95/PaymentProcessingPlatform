using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.DTOs;
using Paymentprocess.Api.Models;
namespace Paymentprocess.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customers entity
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Firstname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Lastname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(265);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreationDate).IsRequired();
            });

            // Payments entity
            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.CustomerID).IsRequired();
                entity.Property(e => e.MerchantReference).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OriginalPaymentAmount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.OutstandingAmount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.PaymentStatus).IsRequired().HasMaxLength(50).HasConversion<string>();
                entity.Property(e => e.CreationDate).IsRequired();

                // One Customer can have many payments
                entity.HasOne(e => e.Customer)
                .WithMany(p => p.Payments)
                .HasForeignKey(e => e.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.CustomerID);
            });

            // Repayment entity
            modelBuilder.Entity<Repayment>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.RepaymentAmount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(e => e.ExternalRefrence)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreationDate)
                    .IsRequired();

                entity.HasOne(e => e.Payment)
                    .WithMany(p => p.Repayments)
                    .HasForeignKey(e => e.PaymentIdentifier)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.RepaymentStatus)
                    .HasConversion<string>()
                    .IsRequired();

                entity.HasIndex(e => e.PaymentIdentifier);
            });

            // Transactions entity
            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.TransactionType).HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
                entity.Property(e => e.Reference).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.CreationDate).IsRequired();

                // One payment can contain multiple transactions
                entity.HasOne(e => e.Payment)
                .WithMany(p => p.Transactions)
                .HasForeignKey(e => e.PaymentID)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PaymentID);
            });

            modelBuilder.Entity<IdempotencyRecord>(entity =>
            {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.IdempotencyKey)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.RequestHash)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.CreationDate)
                    .IsRequired();

                // Every idempotency key must be unique
                entity.HasIndex(e => e.IdempotencyKey)
                    .IsUnique();

                entity.HasOne(e => e.Repayment)
                    .WithMany()
                    .HasForeignKey(e => e.RepaymentID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public DbSet<Customers> Customers { get; set; } = null!;
        public DbSet<Payments> Payments { get; set; } = null!;

        public DbSet<Repayment> Repayments { get; set; } = null!;

        public DbSet<Transactions> Transactions { get; set; } = null!;

        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;
    }
}
