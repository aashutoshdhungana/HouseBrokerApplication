using HouseBrokerApplication.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApplication.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
