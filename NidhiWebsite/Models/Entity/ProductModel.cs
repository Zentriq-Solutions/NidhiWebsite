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
        [ForeignKey(nameof(User))]
        public int product_user_id { get; set; }
        [ValidateNever]
        public virtual User Data_tbl_User { get; set; }
        public DateTime product_created_date { get; set; }
        public DateTime product_row_date { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
