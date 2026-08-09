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

        #region Delete User


        [HttpPost("DeleteUser")]
        public async Task<IActionResult> Delete(int userId)
        {
            try
            {
                var itemgroup = await datacontext.Data_tbl_User.
                                Where(x => x.user_id == userId).
                                ExecuteDeleteAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        [HttpPost("saveuser")]
        public async Task<IActionResult> savecustomer(User data)
        {
            try
            {
              
                bool exists = await datacontext.Data_tbl_User.AsNoTracking()
                                .AnyAsync(l =>l.user_name == data.user_name && l.user_phone_number == data.user_phone_number && l.user_id != data.user_id);

                if (exists)
                {
                    return BadRequest("Name Already Exist");
                }
             
                data.user_password = BCrypt.Net.BCrypt.HashPassword(data.user_password);
                var userdata = new User();
                if (data.user_id==0)
                {
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
                        user_full_name = data.user_full_name,
                    };
                    datacontext.Data_tbl_User.Add(userdata);
                }
                else
                {
                     userdata = await datacontext.Data_tbl_User.FirstOrDefaultAsync(m => m.user_id == data.user_id);
                    if (userdata != null)
                    {
                        userdata.user_name = data.user_name;
                        //userdata.user_password = data.user_password;
                        userdata.user_email = data.user_email;
                        userdata.user_phone_number = data.user_phone_number;
                        userdata.user_place = data.user_place;
                        userdata.user_pincode = data.user_pincode;
                        userdata.user_address = data.user_address;
                        userdata.user_row_date = DateTime.UtcNow;
                        //userdata.user_is_admin = data.user_is_admin;
                        userdata.user_full_name = data.user_full_name;
                    }
                }
                await datacontext.SaveChangesAsync();
                return Ok(userdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uservalidation")]
        public async Task<IActionResult> UserValidation(AuthenticationModel data)
        {
            try
            {
                var user = await datacontext.Data_tbl_User.FirstOrDefaultAsync(x =>x.user_name == data.user_name);
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

        [HttpPost("userDetails")]
        public async Task<IActionResult> UserDetails(int userId)
        {
            try
            {
                var userdata=await datacontext.Data_tbl_User.Where(l=> l.user_id == userId).
                             Select(m=>new
                             {
                                 user_id=m.user_id,
                                 user_full_name =m.user_full_name,
                                 user_address = m.user_address,
                                 user_email = m.user_email,
                                 user_phone_number = m.user_phone_number,
                                 user_pincode = m.user_pincode,
                                 user_name = m.user_name,
                                 user_place = m.user_place,
                             }).FirstOrDefaultAsync();
                return Ok(userdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
