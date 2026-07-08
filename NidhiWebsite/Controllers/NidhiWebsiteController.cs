using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult savevouchertypeitems(User data)
        {
            try
            {
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
                bool isvalidate = datacontext.Data_tbl_User.Any(x =>
                    x.user_name == data.user_name &&
                    x.user_password == data.user_password);

                if (!isvalidate)
                {
                    return Unauthorized("Invalid username or password.");
                }
                var isadmin = datacontext.Data_tbl_User.Where(l => l.user_name == data.user_name && l.user_password == data.user_password).Select(k =>new { k.user_is_admin,k.user_id }).FirstOrDefault();
                return Ok(isadmin);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
