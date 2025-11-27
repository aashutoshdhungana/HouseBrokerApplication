using HouseBrokerApplication.Domain.Aggregates.Listing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBrokerApplication.Infrastructure.EntityConfigurations
{
    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            builder.HasOne(l => l.Broker)
                   .WithMany()
                   .HasForeignKey(l => l.BrokerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(l => l.Address);

            builder.HasMany(l => l.Images)
                .WithOne(l => l.Listing)
                .HasForeignKey(li => li.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.Offers)
                .WithOne(o => o.Listing)
                .HasForeignKey(o => o.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.Deals)
                .WithOne(d => d.Listing)
                .HasForeignKey(d => d.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(l => l.Price)
                   .HasPrecision(18, 2);
        }
    }
}
