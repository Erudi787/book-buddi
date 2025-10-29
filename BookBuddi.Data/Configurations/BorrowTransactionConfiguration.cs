using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class BorrowTransactionConfiguration : IEntityTypeConfiguration<BorrowTransaction>
    {
        public void Configure(EntityTypeBuilder<BorrowTransaction> builder)
        {
            builder.HasKey(bt => bt.TransactionId);

            builder.Property(bt => bt.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(bt => bt.Notes)
                .HasMaxLength(1000);

            builder.Property(bt => bt.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(bt => bt.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(bt => bt.BookId);
            builder.HasIndex(bt => bt.MemberId);
            builder.HasIndex(bt => bt.Status);

            // Foreign key relationships
            builder.HasOne<Book>()
                .WithMany()
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Member>()
                .WithMany()
                .HasForeignKey(bt => bt.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
