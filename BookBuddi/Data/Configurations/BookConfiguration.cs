using BookBuddi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // primary key
            builder.HasKey(b => b.BookId);

            // properties
            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(13);

            builder.Property(b => b.BookTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>();

            // idx for performance
            builder.HasIndex(b => b.ISBN);
            builder.HasIndex(b => b.BookTitle);
            builder.HasIndex(b => b.CategoryId);
            builder.HasIndex(b => b.GenreId);

            // relationships
            builder.HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BookAuthors)
                .WithOne(ba => ba.Book)
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.BorrowTransactions)
                .WithOne(bt => bt.Book)
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}