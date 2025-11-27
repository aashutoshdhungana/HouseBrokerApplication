using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using HouseBrokerApplication.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBrokerApplication.Infrastructure.EntityConfigurations
{
    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.HasOne<AppUser>()
                .WithOne(a => a.UserInfo)
                .HasForeignKey<AppUser>(a => a.UserInfoId);
        }
    }
}
