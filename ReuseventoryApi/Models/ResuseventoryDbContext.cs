using Microsoft.EntityFrameworkCore;

namespace ReuseventoryApi.Models
{
    public class ReuseventoryDbContext : DbContext
    {
        public ReuseventoryDbContext(DbContextOptions<ReuseventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingTag> ListingTags { get; set; }
    }
}