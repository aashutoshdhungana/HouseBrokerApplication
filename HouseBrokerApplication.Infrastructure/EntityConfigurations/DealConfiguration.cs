using HouseBrokerApplication.Domain.Aggregates.Listing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBrokerApplication.Infrastructure.EntityConfigurations
{
    public class DealConfiguration : IEntityTypeConfiguration<Deal>
    {
        public void Configure(EntityTypeBuilder<Deal> builder)
        {
            builder.HasOne(d => d.Offer)
                .WithOne(o => o.Deal)
                .HasForeignKey<Deal>(d => d.OfferId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(d => d.Commission)
                .HasPrecision(18, 2);
        }
    }
}
