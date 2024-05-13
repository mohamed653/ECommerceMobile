using ECommereceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Data
{
    public partial class ECommerceContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.IsDeleted == false);
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsDeleted == false);
        }

    }
}
