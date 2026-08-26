using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NidhiWebsite.Data;
using NidhiWebsite.Models.Entity;

namespace NidhiWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext datacontext;
        private readonly IMemoryCache _cache;

        // Centralize cache keys so you never typo one when invalidating
        private const string ProductListCacheKey = "product_list";
        private const string AllProductCacheKey = "all_product_list";
        private const string ItemGroupCacheKey = "itemGroups_list";
        private static string GroupWiseCacheKey(int groupId) => $"product_group_{groupId}";

        public ProductController(IWebHostEnvironment environment, ApplicationDbContext context, IMemoryCache cache)
        {
            _environment = environment;
            datacontext = context;
            _cache = cache;
        }

        // ---------- Shared cache helper (avoids repeating the same TryGetValue/Set block everywhere) ----------
        private async Task<T> GetOrSetCacheAsync<T>(string key, Func<Task<T>> factory, int minutes = 10)
        {
            if (_cache.TryGetValue(key, out T cached))
                return cached;

            var data = await factory();
            var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes));
            _cache.Set(key, data, options);
            return data;
        }
        #region Delete Product


        [HttpPost("DeleteProduct")]
        public async Task<IActionResult> Delete(int productid)
        {
            try
            {
                bool isrefcart = await datacontext.Data_tbl_Cart.AnyAsync(x => x.cart_product_id == productid);
                if (isrefcart)
                {
                    return BadRequest("Product is referenced in cart.");
                }
                bool isrefwishlist = await datacontext.Data_tbl_Wish_list.AnyAsync(x => x.wishlist_product_id == productid);
                if (isrefwishlist)
                {
                    return BadRequest("Product is referenced in wishlist.");
                }
                var itemgroup = await datacontext.Data_tbl_Product.
                                Where(x => x.product_id == productid).
                                ExecuteDeleteAsync();
                _cache.Remove(ProductListCacheKey);
                _cache.Remove(AllProductCacheKey);
                return Ok("Successfully Deleted Item");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        [HttpPost("AddProduct")]
        public async Task<IActionResult> Create(ProductModel product)
        {
            try
            {
                if (product.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Upload");

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }

                    product.product_image = fileName;
                }

                var issaved = await SaveProductAsync(product);
                return issaved ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<bool> SaveProductAsync(ProductModel product)
        {
            try
            {
                // ONE query instead of two - checks both name and code duplication in a single round trip
                bool isDuplicate = await datacontext.Data_tbl_Product.AsNoTracking()
                    .Where(l => l.product_id != 0 && l.product_id != product.product_id)
                    .AnyAsync(l => l.product_name == product.product_name || l.product_code == product.product_code);

                if (isDuplicate)
                    return false;

                if (product.product_id == 0)
                {
                    var productdata = new ProductModel
                    {
                        product_name = product.product_name,
                        product_code = product.product_code,
                        product_image = product.product_image,
                        product_price = product.product_price,
                        product_description = product.product_description,
                        product_user_id = product.product_user_id,
                        product_created_date = DateTime.UtcNow,
                        product_row_date = DateTime.UtcNow,
                        product_item_group_id = product.product_item_group_id,
                    };
                    datacontext.Data_tbl_Product.Add(productdata);
                }
                else
                {
                    var productdata = await datacontext.Data_tbl_Product.FirstOrDefaultAsync(m => m.product_id == product.product_id);
                    if (productdata != null)
                    {
                        productdata.product_name = product.product_name;
                        productdata.product_code = product.product_code;
                        productdata.product_image = product.product_image;
                        productdata.product_price = product.product_price;
                        productdata.product_description = product.product_description;
                        productdata.product_user_id = product.product_user_id;
                        productdata.product_row_date = DateTime.UtcNow;
                        productdata.product_item_group_id = product.product_item_group_id;
                    }
                }

                await datacontext.SaveChangesAsync();

                // Invalidate every cache entry that could now be stale
                _cache.Remove(ProductListCacheKey);
                _cache.Remove(AllProductCacheKey);
                //if (product.product_item_group_id.HasValue)
                //    _cache.Remove(GroupWiseCacheKey(product.product_item_group_id.Value));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct()
        {
            try
            {
                var productdata = await GetOrSetCacheAsync(ProductListCacheKey, async () =>
                {
                    var path = "/Upload/";
                    return await datacontext.Data_tbl_Product.AsNoTracking()
                        .Where(l => l.product_id != 0)
                        .OrderByDescending(m => m.product_id)
                        .Select(m => new ProductForInitailloadingModel
                        {
                            productid = m.product_id,
                            name = m.product_name,
                            image = path + m.product_image,
                            price = m.product_price,
                        }).Take(12).ToListAsync();
                });

                return Ok(productdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllProductItemGroupWise")]
        public async Task<IActionResult> GetAllProductItemGroupWise(int itemgroupid)
        {
            try
            {
                var productdata = await GetOrSetCacheAsync(GroupWiseCacheKey(itemgroupid), async () =>
                {
                    var path = "/Upload/";
                    return await datacontext.Data_tbl_Product.AsNoTracking()
                        .Where(l => l.product_id != 0 && l.product_item_group_id == itemgroupid)
                        .Select(m => new
                        {
                            productid = m.product_id,
                            name = m.product_name,
                            code = m.product_code,
                            image = path + m.product_image,
                            price = m.product_price,
                            description = m.product_description,
                        }).ToListAsync();
                });

                return Ok(productdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Not cached - result depends on the specific user's cart, so it's inherently per-request
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int productId, int userId)
        {
            try
            {
                var path = "/Upload/";

                var product = await datacontext.Data_tbl_Product.AsNoTracking()
                    .Where(p => p.product_id == productId)
                    .Select(p => new
                    {
                        productid = p.product_id,
                        name = p.product_name,
                        price = p.product_price,
                        description = p.product_description,
                        code = p.product_code,
                        image = path + p.product_image,
                        iscartitem = userId > 0 &&
                             p.Data_tbl_Cart.Any(c => c.cart_user_id == userId)
                    }).FirstOrDefaultAsync();

                if (product == null)
                    return NotFound(new { message = "Product not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var productdata = await GetOrSetCacheAsync(AllProductCacheKey, async () =>
                {
                    return await datacontext.Data_tbl_Product.AsNoTracking()
                        .Where(l => l.product_id != 0)
                        .Select(m => new
                        {
                            productid = m.product_id,
                            name = m.product_name,
                            code = m.product_code,
                            description = m.product_description,
                            price = m.product_price,
                        }).ToListAsync();
                });

                return Ok(productdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetProductByIdForAdmin")]
        public async Task<IActionResult> GetProductByIdForAdmin(int productId)
        {
            try
            {
                var itemgroup = await datacontext.Data_tbl_Product.AsNoTracking()
                    .Where(l => l.product_id == productId)
                    .Select(m => new
                    {
                        product_id = m.product_id,
                        product_name = m.product_name,
                        product_code = m.product_code,
                        product_price = m.product_price,
                        product_description = m.product_description,
                        product_image = m.product_image,
                        product_item_group_id = m.product_item_group_id,
                        product_item_group_name = m.Data_tbl_Item_group.item_group_name,
                    }).FirstOrDefaultAsync();
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ItemGroupForProvider")]
        public async Task<IActionResult> GetItemGroupForProvider()
        {
            try
            {
                var itemgroup = await GetOrSetCacheAsync(ItemGroupCacheKey, async () =>
                {
                    return await datacontext.Data_tbl_Item_group.AsNoTracking()
                        .Where(l => l.item_group_id != 0)
                        .Select(m => new ItemGroupModelForProvider
                        {
                            item_group_id = m.item_group_id,
                            item_group_name = m.item_group_name,
                        }).ToListAsync();
                });

                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllProductCount")]
        public async Task<IActionResult> GetAllProductCount()
        {
            try
            {
                var count = await datacontext.Data_tbl_Product.Where(l => l.product_id != 0).CountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetViewAllProducts")]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 15, string sort = "")
        {
            try
            {
                var path = "/Upload/";

                if (sort == "asc")
                {
                    var productsAsc = await datacontext.Data_tbl_Product
                        .AsNoTracking()
                        .Where(p => p.product_id != 0)
                        .OrderBy(p => p.product_price)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => new
                        {
                            id = p.product_id,
                            name = p.product_name,
                            code = p.product_code,
                            price = p.product_price,
                            image = path + p.product_image,
                            description = p.product_description
                        })
                        .ToListAsync();
                    return Ok(productsAsc);
                }
                else if (sort == "desc")
                {
                    var productsDesc = await datacontext.Data_tbl_Product
                        .AsNoTracking()
                        .Where(p => p.product_id != 0)
                        .OrderByDescending(p => p.product_price)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(p => new
                        {
                            id = p.product_id,
                            name = p.product_name,
                            code = p.product_code,
                            price = p.product_price,
                            image = path + p.product_image,
                            description = p.product_description
                        })
                        .ToListAsync();
                    return Ok(productsDesc);
                }
                var products = await datacontext.Data_tbl_Product
                    .AsNoTracking()
                    .Where(p => p.product_id != 0)
                    .OrderBy(p => p.product_id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        id = p.product_id,
                        name = p.product_name,
                        code = p.product_code,
                        price = p.product_price,
                        image = path + p.product_image,
                        description = p.product_description
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}