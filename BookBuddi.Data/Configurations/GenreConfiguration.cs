using BookBuddi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBuddi.Data.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasKey(g => g.GenreId);

            builder.Property(g => g.GenreName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.GenreDescription)
                .HasMaxLength(500);

            builder.Property(g => g.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(g => g.GenreName)
                .IsUnique();
        }
    }
}
