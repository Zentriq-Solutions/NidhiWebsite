using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NidhiWebsite.Data;
using NidhiWebsite.Models.Entity;
namespace NidhiWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NidhiWebsiteController : ControllerBase
    {
        private readonly ApplicationDbContext datacontext;
        public NidhiWebsiteController(ApplicationDbContext datacontext)
        {
            this.datacontext = datacontext;
        }
        [HttpPost("saveuser")]
        public IActionResult savecustomer(User data)
        {
            try
            {
                var alluser = datacontext.Data_tbl_User.AsNoTracking().
                                 Where(l => l.user_id != 0).
                                 Select(m => new
                                 {
                                     user_id = m.user_id,
                                     user_name = m.user_name,
                                     user_phone_number = m.user_phone_number,
                                 });
                if (alluser.Any(l => l.user_name == data.user_name && l.user_phone_number == data.user_phone_number/* l.product_id != product.product_id*/))
                {
                    return BadRequest("Name Already Exist");
                }
                data.user_password = BCrypt.Net.BCrypt.HashPassword(data.user_password);
                var userdata = new User();
                userdata = new User
                {
                    user_name = data.user_name,
                    user_password = data.user_password,
                    user_email = data.user_email,
                    user_phone_number = data.user_phone_number,
                    user_place = data.user_place,
                    user_pincode = data.user_pincode,
                    user_address = data.user_address,
                    user_row_date = DateTime.UtcNow,
                    user_is_admin = data.user_is_admin,

                };
                datacontext.Data_tbl_User.Add(userdata);
                datacontext.SaveChanges();
                return Ok(userdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uservalidation")]
        public IActionResult UserValidation(AuthenticationModel data)
        {
            try
            {
                var user = datacontext.Data_tbl_User.FirstOrDefault(x =>
                    x.user_name == data.user_name);
                if (user == null)
                {
                    return Unauthorized("Invalid username.");
                }
                bool isvalidate = BCrypt.Net.BCrypt.Verify(
                                data.user_password,
                                user.user_password);
               if (!isvalidate)
                {
                    return Unauthorized("Invalid username or password.");
                }
                   return Ok(new
                   {
                       user.user_is_admin,
                       user.user_id
                   });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
