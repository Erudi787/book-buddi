using BookBuddi.Models;
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

            builder.HasIndex(f => f.MemberId);
            builder.HasIndex(f => f.Status);
            builder.HasIndex(f => f.TransactionId)
                .IsUnique();
        }
    }
}