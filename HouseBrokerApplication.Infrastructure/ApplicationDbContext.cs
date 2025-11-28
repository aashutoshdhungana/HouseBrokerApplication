using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Domain.Aggregates.GlobalConfig;
using HouseBrokerApplication.Domain.Aggregates.Listing;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using HouseBrokerApplication.Domain.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApplication.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>, IUnitOfWork
    {
        private readonly ICurrentUserService _currentUserService;
        public ApplicationDbContext(DbContextOptions options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<ListingImage> ListingImages { get; set; } = null!;
        public DbSet<Offer> Offers { get; set; } = null!;
        public DbSet<Deal> Deals { get; set; } = null!;
        public DbSet<Domain.Aggregates.FileInfo.FileInfo> Files { get; set; } = null!;
        public DbSet<UserInfo> UserInfos { get; set; } = null!;
        public DbSet<CommissionConfig> CommissionConfigs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);
        }

        public new async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var actionByUserId = _currentUserService.UserId;
                if (actionByUserId.HasValue)
                {
                    foreach (var entry in ChangeTracker.Entries<AuditedEntity>())
                    {
                        switch (entry.State)
                        {
                            case EntityState.Added:
                                entry.Entity.SetCreated(actionByUserId.Value);
                                break;
                            case EntityState.Modified:
                                entry.Entity.SetUpdated(actionByUserId.Value);
                                break;
                        }
                    }
                }
                var changes = await base.SaveChangesAsync(cancellationToken);
                return changes > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
