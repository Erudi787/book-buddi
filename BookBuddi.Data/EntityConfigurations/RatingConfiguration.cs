using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookBuddi.Data.Models;

namespace BookBuddi.Data.EntityConfigurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Ratings");

            builder.HasKey(r => r.RatingId);

            builder.Property(r => r.Score)
                .IsRequired()
                .HasComment("Rating score from 1 to 5 stars");

            builder.Property(r => r.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.CreatedTime)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.UpdatedBy)
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(r => r.Book)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Member)
                .WithMany()
                .HasForeignKey(r => r.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one rating per book per member
            builder.HasIndex(r => new { r.BookId, r.MemberId })
                .IsUnique()
                .HasDatabaseName("IX_Ratings_BookId_MemberId");

            // Index for performance
            builder.HasIndex(r => r.BookId)
                .HasDatabaseName("IX_Ratings_BookId");

            builder.HasIndex(r => r.MemberId)
                .HasDatabaseName("IX_Ratings_MemberId");
        }
    }
}
