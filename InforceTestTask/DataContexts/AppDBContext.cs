using InforceTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace InforceTestTask.DataContexts
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ShortUrl> ShortUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            builder.Entity<ShortUrl>()
                .HasIndex(url => url.Url)
                .IsUnique();
        }
    }
}
