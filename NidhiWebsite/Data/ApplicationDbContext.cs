
using NidhiWebsite.Models.Entity;
using Microsoft.EntityFrameworkCore;


namespace NidhiWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Data_tbl_User { get; set; }
        public DbSet<ProductModel> Data_tbl_Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Data_tbl_User)
                .WithMany(u => u.Data_tbl_Product)
                .HasForeignKey(p => p.product_user_id)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
