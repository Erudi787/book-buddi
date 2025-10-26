using BookBuddi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            // composite primary key
            builder.HasKey(ba => new { ba.BookId, ba.AuthorId });

            builder.Property(ba => ba.AuthorOrder)
                .HasDefaultValue(1);
        }
    }
}