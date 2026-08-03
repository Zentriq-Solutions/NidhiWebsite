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
        private const string path = "/Upload/";
        public CustomerController(IWebHostEnvironment environment, ApplicationDbContext context, IMemoryCache cache)
        {
            _environment = environment;
            datacontext = context;
            _cache = cache;
        }
        #region WishListApis
        [HttpGet("GetWishList")]
        public async Task<IActionResult> GetWishList(int userId)
        {
            try
            {
                var wishlistdata = await datacontext.Data_tbl_Wish_list.AsNoTracking()
                  .Where(w => w.wishlist_user_id == userId)
                  .Select(m => new WishListForInitialLoadingModel
                  {
                      productid = m.wishlist_product_id,
                  }).Take(20).ToListAsync();
                return Ok(wishlistdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SaveWishList")]
        public async Task<IActionResult> SaveWishList(int userId, int productId)
        {
            try
            {
                var exists = await datacontext.Data_tbl_Wish_list
                    .AnyAsync(x => x.wishlist_user_id == userId &&
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
                await datacontext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("WishListRemove")]
        public async Task<IActionResult> RemovewishList(int userId, int productId)
        {
            try
            {
                var rowsAffected = await datacontext.Data_tbl_Wish_list
                        .Where(x => x.wishlist_user_id == userId && x.wishlist_product_id == productId)
                        .ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    return NotFound("Wishlist item not found.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllWishList")]
        public async Task<IActionResult> GetAllWishList(int userId)
        {
            try
            {
                var wishlistdata = await datacontext.Data_tbl_Wish_list
                    .AsNoTracking()
                    .Where(w => w.wishlist_user_id == userId)
                    .Select(m => new Srvc_GetAllWishList_Model
                    {
                        ProductID = m.wishlist_product_id,
                        ProductName = m.Data_tbl_Product.product_name,
                        ProductUserId = m.wishlist_user_id,
                        ProductImage = path + m.Data_tbl_Product.product_image,
                        ProductPrice = m.Data_tbl_Product.product_price,
                    }).ToListAsync();

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
        public async Task<IActionResult> SaveCart(int userId, int productId)
        {
            try
            {
                var exists = await datacontext.Data_tbl_Cart.AsNoTracking()
                 .AnyAsync(x => x.cart_user_id == userId && x.cart_product_id == productId);

                if (exists)
                {
                    return Ok("Already Exist");
                }

                var cart = new CartModel
                {
                    cart_user_id = userId,
                    cart_product_id = productId,
                    cart_row_date = DateTime.UtcNow
                };

                datacontext.Data_tbl_Cart.Add(cart);
                await datacontext.SaveChangesAsync();

                return Ok("Product added to cart");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CartRemove")]
        public async Task<IActionResult> RemoveCart(int userId, int productId)
        {
            try
            {
                var rowsAffected = await datacontext.Data_tbl_Cart
            .Where(x => x.cart_user_id == userId && x.cart_product_id == productId)
            .ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    return NotFound("Cart item not found.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllCartItems")]
        public async Task<IActionResult> GetAllGetAllCartItems(int userId)
        {
            try
            {
                var cartdata = await datacontext.Data_tbl_Cart.AsNoTracking()
               .Where(w => w.cart_user_id == userId)
               .Select(m => new Srvc_GetAllCartItems_Model
               {
                   ProductID = m.cart_product_id,
                   ProductName = m.Data_tbl_Product.product_name,
                   ProductImage = path + m.Data_tbl_Product.product_image,
                   ProductPrice = m.Data_tbl_Product.product_price,
               }).ToListAsync();
                return Ok(cartdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
