using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NidhiWebsite.Models.Entity
{

    public class WishListModel
    {
        [Key]
        public int wishlist_id { get; set; }

        public int wishlist_user_id { get; set; }

        [ValidateNever]
        public virtual User Data_tbl_User { get; set; }

        public int wishlist_product_id { get; set; }

        [ValidateNever]
        public virtual ProductModel Data_tbl_Product { get; set; }

        public DateTime wishlist_row_date { get; set; }
    }
    public class WishListForInitialLoadingModel
    {
        public int wishlistid { get; set; }

        public int productid { get; set; }

        public string name { get; set; }

        public string image { get; set; }

        public decimal price { get; set; }

    }
}