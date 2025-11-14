using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(p => p.TokenId);

            builder.Property(p => p.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.CreatedTime)
                .IsRequired();

            builder.Property(p => p.ExpiryTime)
                .IsRequired();

            builder.Property(p => p.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(p => p.Member)
                .WithMany()
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.Token);
        }
    }
}
