using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NidhiWebsite.Models.Entity
{
    public class ItemGroupModel
    {
        [Key]
        public int item_group_id { get; set; }

        public string item_group_name { get; set; }

        public string item_group_code { get; set; }

        public string? item_group_image { get; set; }

        public string item_group_description { get; set; }

        public int item_group_user_id { get; set; }

        [ValidateNever]
        public virtual User Data_tbl_User { get; set; }

        public DateTime item_group_row_date { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [ValidateNever]
        public ICollection<ProductModel> Data_tbl_Product { get; set; }
            = new List<ProductModel>();
    }
    public class ItemGroupModelForProvider
    {
        public int item_group_id { get; set; }
        public string item_group_name { get; set; }
    }
}