using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class FineConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.HasKey(f => f.FineId);

            builder.Property(f => f.Amount)
                .IsRequired()
                .HasColumnType("decimal(10, 2)");

            builder.Property(f => f.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(f => f.Reason)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(f => f.Notes)
                .HasMaxLength(1000);

            builder.Property(f => f.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(f => f.MemberId);
            builder.HasIndex(f => f.Status);

            // Foreign key relationships
            builder.HasOne<BorrowTransaction>()
                .WithMany()
                .HasForeignKey(f => f.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Member>()
                .WithMany()
                .HasForeignKey(f => f.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
