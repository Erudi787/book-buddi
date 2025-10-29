using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class BookRequestConfiguration : IEntityTypeConfiguration<BookRequest>
    {
        public void Configure(EntityTypeBuilder<BookRequest> builder)
        {
            builder.HasKey(br => br.RequestId);

            builder.Property(br => br.BookTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(br => br.AuthorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(br => br.ISBN)
                .HasMaxLength(13);

            builder.Property(br => br.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(br => br.AdminNotes)
                .HasMaxLength(1000);

            builder.Property(br => br.MemberNotes)
                .HasMaxLength(1000);

            builder.Property(br => br.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(br => br.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(br => br.MemberId);
            builder.HasIndex(br => br.Status);

            // Foreign key relationships
            builder.HasOne<Member>()
                .WithMany()
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
