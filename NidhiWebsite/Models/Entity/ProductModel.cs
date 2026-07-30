using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NidhiWebsite.Models.Entity
{
    public class ProductModel
    {
        [Key]
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_code { get; set; }
        public string? product_image { get; set; }
        public decimal product_price { get; set; }
        public string product_description { get; set; }
        public DateTime product_created_date { get; set; }
        public DateTime product_row_date { get; set; }
        public int product_user_id { get; set; }

        [ForeignKey(nameof(product_user_id))]
        [ValidateNever]
        public virtual User Data_tbl_User { get; set; }
        [ValidateNever]
        public ICollection<WishListModel> Data_tbl_Wish_list { get; set; }
    = new List<WishListModel>();
        public int product_item_group_id { get; set; }

        [ForeignKey(nameof(product_item_group_id))]
        [ValidateNever]
        public virtual ItemGroupModel Data_tbl_Item_group { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }

    public class ProductForInitailloadingModel
    {
        public int productid { get; set; }
        public string name { get; set; } 
        public string image { get; set; }
        public decimal price { get; set; }
    }
}
