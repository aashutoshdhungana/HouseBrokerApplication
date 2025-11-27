using HouseBrokerApplication.Domain.Aggregates.GlobalConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBrokerApplication.Infrastructure.EntityConfigurations
{
    public class CommissionConfigConfiguration : IEntityTypeConfiguration<CommissionConfig>
    {
        public void Configure(EntityTypeBuilder<CommissionConfig> builder)
        {
            builder.Property(cc => cc.CommissionRate)
                   .HasPrecision(5, 2);
            builder.Property(cc => cc.StartingPrice)
                   .HasPrecision(18, 2);
            builder.Property(cc => cc.EndingPrice)
                   .HasPrecision(18, 2);
        }
    }
}
