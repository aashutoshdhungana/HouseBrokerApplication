using HouseBrokerApplication.Domain.Aggregates.Listing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBrokerApplication.Infrastructure.EntityConfigurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.OfferAmount)
                .HasPrecision(18, 2);
        }
    }
}
