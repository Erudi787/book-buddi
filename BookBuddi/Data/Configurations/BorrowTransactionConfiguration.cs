using BookBuddi.Models;
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

            builder.HasIndex(bt => bt.BookId);
            builder.HasIndex(bt => bt.MemberId);
            builder.HasIndex(bt => bt.Status);
            builder.HasIndex(bt => bt.DueDate);

            builder.HasOne(bt => bt.Fine)
                .WithOne(f => f.BorrowTransaction)
                .HasForeignKey<Fine>(f => f.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}