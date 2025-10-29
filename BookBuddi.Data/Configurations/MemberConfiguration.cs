using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasKey(m => m.MemberId);

            builder.Property(m => m.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.Phone)
                .HasMaxLength(20);

            builder.Property(m => m.Address)
                .HasMaxLength(500);

            builder.Property(m => m.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(m => m.BorrowingLimit)
                .HasDefaultValue(5);

            builder.Property(m => m.CurrentBorrowedCount)
                .HasDefaultValue(0);

            builder.Property(m => m.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(m => m.Email)
                .IsUnique();
        }
    }
}
