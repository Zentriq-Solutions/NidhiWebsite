
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
        public DbSet<ItemGroupModel> Data_tbl_Item_group { get; set; }
        public DbSet<WishListModel> Data_tbl_Wish_list { get; set; }
        public DbSet<CartModel> Data_tbl_Cart{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Data_tbl_User)
                .WithMany(u => u.Data_tbl_Product)
                .HasForeignKey(p => p.product_user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemGroupModel>()
                .HasOne(i => i.Data_tbl_User)
                .WithMany(u => u.Data_tbl_Item_group)
                .HasForeignKey(i => i.item_group_user_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemGroupModel>()
                .HasMany(i => i.Data_tbl_Product)
                .WithOne(p => p.Data_tbl_Item_group)
                .HasForeignKey(p => p.product_item_group_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WishListModel>()
                .HasOne(w => w.Data_tbl_User)
                .WithMany(u => u.Data_tbl_Wish_list)
                .HasForeignKey(w => w.wishlist_user_id)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<WishListModel>()
                .HasOne(w => w.Data_tbl_Product)
                .WithMany(p => p.Data_tbl_Wish_list)
                .HasForeignKey(w => w.wishlist_product_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartModel>()
                .HasOne(w => w.Data_tbl_User)
                .WithMany(u => u.Data_tbl_Cart)
                .HasForeignKey(w => w.cart_user_id)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CartModel>()
                .HasOne(w => w.Data_tbl_Product)
                .WithMany(p => p.Data_tbl_Cart)
                .HasForeignKey(w => w.cart_product_id)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
