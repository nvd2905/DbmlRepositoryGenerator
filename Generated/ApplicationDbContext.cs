using Microsoft.EntityFrameworkCore;
using Inventory.Entities;

namespace Inventory
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Users> Userss { get; set; }
        
        public DbSet<Posts> Postss { get; set; }
        
        public DbSet<Comments> Commentss { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // Configure Posts -> Users relationship
            modelBuilder.Entity<Posts>()
                .HasOne<Users>()
                .WithOne()
                .HasForeignKey("user_id");
            
            // Configure Comments -> Posts relationship
            modelBuilder.Entity<Comments>()
                .HasOne<Posts>()
                .WithOne()
                .HasForeignKey("post_id");
            
            // Configure Comments -> Users relationship
            modelBuilder.Entity<Comments>()
                .HasOne<Users>()
                .WithOne()
                .HasForeignKey("user_id");
            
        }
    }
}