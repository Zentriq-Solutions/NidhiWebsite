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
        #region WishListApis
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

        [HttpGet("GetAllWishList")]
        public IActionResult GetAllWishList(int userId)
        {
            try
            {
                var path="/Upload/";
                var wishlistdata = datacontext.Data_tbl_Wish_list.AsNoTracking()
                  .Where(w => w.wishlist_user_id == userId)
                  .Select(m => new Srvc_GetAllWishList_Model
                  {
                      ProductID = m.wishlist_product_id,
                      ProductName = m.Data_tbl_Product.product_name,
                      ProductUserId = m.wishlist_user_id,
                      ProductImage = path+ m.Data_tbl_Product.product_image,
                      ProductPrice = m.Data_tbl_Product.product_price,
                  }).ToList();
                return Ok(wishlistdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Cart Apis
        [HttpPost("SaveCart")]
        public IActionResult SaveCart(int userId, int productId)
        {
            try
            {
                var exists = datacontext.Data_tbl_Cart.Any(x => x.cart_user_id == userId &&x.cart_product_id == productId);
                if (exists)
                {
                    return Ok("Already  Exist");
                }
                CartModel cart = new CartModel
                {
                    cart_user_id = userId,
                    cart_product_id = productId,
                    cart_row_date = DateTime.UtcNow
                };
                datacontext.Data_tbl_Cart.Add(cart);
                datacontext.SaveChanges();
                return Ok("Product added to cart");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CartRemove")]
        public IActionResult RemoveCart(int cartid)
        {
            try
            {
                var exists = datacontext.Data_tbl_Cart
                    .FirstOrDefault(x => x.cart_id == cartid);
                if (exists != null)
                {
                    datacontext.Data_tbl_Cart.Remove(exists);
                }
                datacontext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllCartItems")]
        public IActionResult GetAllGetAllCartItemsWishList(int userId)
        {
            try
            {
                var path = "/Upload/";
                var wishlistdata = datacontext.Data_tbl_Cart.AsNoTracking()
                  .Where(w => w.cart_user_id == userId)
                  .Select(m => new Srvc_GetAllCartItems_Model
                  {
                      ProductID = m.cart_product_id,
                      ProductName = m.Data_tbl_Product.product_name,
                      ProductImage = path + m.Data_tbl_Product.product_image,
                      ProductPrice = m.Data_tbl_Product.product_price,
                  }).ToList();
                return Ok(wishlistdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
