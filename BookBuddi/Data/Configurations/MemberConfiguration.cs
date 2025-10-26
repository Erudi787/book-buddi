using BookBuddi.Models;
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

            builder.Ignore(m => m.FullName);

            builder.Property(m => m.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(m => m.Email)
                .IsUnique();

            builder.Property(m => m.BorrowingLimit)
                .HasDefaultValue(5);

            builder.Property(m => m.CurrentBorrowedCount)
                .HasDefaultValue(0);

            builder.HasMany(m => m.BorrowTransactions)
                .WithOne(bt => bt.Member)
                .HasForeignKey(bt => bt.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.BookRequests)
                .WithOne(br => br.Member)
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Notifications)
                .WithOne(n => n.Member)
                .HasForeignKey(n => n.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Fines)
                .WithOne(f => f.Member)
                .HasForeignKey(f => f.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}