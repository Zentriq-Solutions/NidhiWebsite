using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NidhiWebsite.Models.Entity
{

    public class WishListModel
    {
        [Key]
        public int wishlist_id { get; set; }
        public DateTime wishlist_row_date { get; set; }

        public int wishlist_user_id { get; set; }

        [ForeignKey(nameof(wishlist_user_id))]
        [ValidateNever]
        public virtual User Data_tbl_User { get; set; }

        public int wishlist_product_id { get; set; }

        [ForeignKey(nameof(wishlist_product_id))]
        [ValidateNever]
        public virtual ProductModel Data_tbl_Product { get; set; }
    }
    public class WishListForInitialLoadingModel
    {
        public int productid { get; set; }

    }

    public class CartModel
    {
        [Key]
        public int cart_id { get; set; }
        public DateTime cart_row_date { get; set; }

        public int cart_user_id { get; set; }

        [ForeignKey(nameof(cart_user_id))]
        [ValidateNever]
        public virtual User Data_tbl_User { get; set; }

        public int cart_product_id { get; set; }

        [ForeignKey(nameof(cart_product_id))]
        [ValidateNever]
        public virtual ProductModel Data_tbl_Product { get; set; }
    }
}