using System.ComponentModel.DataAnnotations;

namespace NidhiWebsite.Models.Entity
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_password { get; set; }
        public string user_email { get; set; }
        public string user_phone_number { get; set; }
        public string user_place { get; set; }
        public int user_pincode{ get; set; }
        public string user_address { get; set; }
        public DateTime user_row_date { get; set; }
        public bool user_is_admin { get; set; }
        public ICollection<ProductModel> Data_tbl_Product { get; set; }
        = new List<ProductModel>();

    }
}
