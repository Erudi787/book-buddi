using BookBuddi.Models;
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
                .HasMaxLength(100);

            builder.Property(br => br.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(br => br.MemberId);
            builder.HasIndex(br => br.Status);
        }
    }
}