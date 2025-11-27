using HouseBrokerApplication.Domain.Aggregates.Listing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBrokerApplication.Infrastructure.EntityConfigurations
{
    public class ListingImageConfiguration : IEntityTypeConfiguration<ListingImage>
    {
        public void Configure(EntityTypeBuilder<ListingImage> builder)
        {
            builder.HasOne(li => li.FileInfo)
                .WithOne()
                .HasForeignKey<ListingImage>(li => li.FileInfoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
