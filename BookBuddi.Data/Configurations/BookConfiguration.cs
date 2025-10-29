using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Primary key
            builder.HasKey(b => b.BookId);

            // Properties
            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(13);

            builder.Property(b => b.BookTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.Publisher)
                .HasMaxLength(200);

            builder.Property(b => b.Description)
                .HasMaxLength(2000);

            builder.Property(b => b.CoverImageUrl)
                .HasMaxLength(500);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(b => b.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes for performance
            builder.HasIndex(b => b.ISBN).IsUnique();
            builder.HasIndex(b => b.BookTitle);
            builder.HasIndex(b => b.CategoryId);
            builder.HasIndex(b => b.GenreId);

            // Relationships (Foreign Keys only)
            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Genre>()
                .WithMany()
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
