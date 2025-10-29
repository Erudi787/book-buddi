using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            // Composite primary key
            builder.HasKey(ba => new { ba.BookId, ba.AuthorId });

            builder.Property(ba => ba.AuthorOrder)
                .HasDefaultValue(1);

            builder.Property(ba => ba.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ba => ba.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            // Foreign key relationships
            builder.HasOne<Book>()
                .WithMany()
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Author>()
                .WithMany()
                .HasForeignKey(ba => ba.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
