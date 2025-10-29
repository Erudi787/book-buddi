using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.NotificationId);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.RelatedEntityType)
                .HasMaxLength(50);

            builder.Property(n => n.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(n => n.MemberId);
            builder.HasIndex(n => n.IsRead);

            // Foreign key relationships
            builder.HasOne<Member>()
                .WithMany()
                .HasForeignKey(n => n.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
