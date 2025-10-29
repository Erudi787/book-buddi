using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.AuthorId);

            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Biography)
                .HasMaxLength(2000);

            builder.Property(a => a.Nationality)
                .HasMaxLength(100);

            builder.Property(a => a.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(a => new { a.FirstName, a.LastName });
        }
    }
}
