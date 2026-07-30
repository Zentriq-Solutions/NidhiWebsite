using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NidhiWebsite.Data;
using NidhiWebsite.Models.Entity;

namespace NidhiWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext datacontext;
        private readonly IMemoryCache _cache;
        public CustomerController(IWebHostEnvironment environment, ApplicationDbContext context, IMemoryCache cache)
        {
            _environment = environment;
            datacontext = context;
            _cache = cache;
        }
        [HttpGet("GetWishList")]
        public  IActionResult GetWishList(int userId)
        {
            try
            {
                var wishlistdata = datacontext.Data_tbl_Wish_list.AsNoTracking()
                  .Where(w => w.wishlist_user_id == userId)
                  .Select(m => new WishListForInitialLoadingModel
                  {
                      productid = m.wishlist_product_id,
                  }).Take(20).ToList();
                return Ok(wishlistdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SaveWishList")]
        public IActionResult SaveWishList(int userId, int productId)
        {
            try
            {
                var exists = datacontext.Data_tbl_Wish_list
                    .Any(x => x.wishlist_user_id == userId &&
                              x.wishlist_product_id == productId);
                if (exists)
                {
                    return Ok(false);
                }
                WishListModel wishlist = new WishListModel
                {
                    wishlist_user_id = userId,
                    wishlist_product_id = productId,
                    wishlist_row_date = DateTime.UtcNow
                };
                datacontext.Data_tbl_Wish_list.Add(wishlist);
                datacontext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("WishListRemove")]
        public IActionResult RemovewishList(int userId, int productId)
        {
            try
            {
                var exists = datacontext.Data_tbl_Wish_list
                    .FirstOrDefault(x => x.wishlist_user_id == userId &&
                              x.wishlist_product_id == productId);
                if(exists != null)
                {
                    datacontext.Data_tbl_Wish_list.Remove(exists);
                }
                datacontext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
