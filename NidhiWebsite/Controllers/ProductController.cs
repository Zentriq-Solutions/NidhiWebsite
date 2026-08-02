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
        public ProductController(IWebHostEnvironment environment, ApplicationDbContext context,IMemoryCache cache)
        {
            _environment = environment;
            datacontext = context;
            _cache=cache;
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> Create(ProductModel product)
        {
            try
            {
                if (product.ImageFile != null)
                {
                    // Create a unique filename
                    string fileName = Guid.NewGuid().ToString() +
                                      Path.GetExtension(product.ImageFile.FileName);

                    // Path to Uploads folder
                    string uploadFolder = Path.Combine(
                        _environment.WebRootPath,
                        "Upload");

                    // Create folder if it doesn't exist
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // Full path of the image
                    string filePath = Path.Combine(uploadFolder, fileName);

                    // Save image to disk
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }

                    // Save filename in database
                    product.product_image = fileName;
                }
                var issaved = SaveProduct(product);
                if (issaved)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool SaveProduct([FromForm] ProductModel product)
        {
            try
            {
                var allproduct = datacontext.Data_tbl_Product.AsNoTracking().
                                 Where(l => l.product_id!= 0).
                                 Select(m => new
                                 {
                                     product_id = m.product_id,
                                     product_name = m.product_name,
                                     product_code = m.product_code,
                                 });
                if (allproduct.Any(l => l.product_name == product.product_name && l.product_id!=product.product_id))
                {
                    return false;
                }

                if (allproduct.Any(l => l.product_code == product.product_code && l.product_id != product.product_id))
                {
                    return false;
                }

                var productdata = new ProductModel();
                productdata = new ProductModel
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
                datacontext.SaveChanges();
                _cache.Remove("product_list");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet("GetProduct")]
        public IActionResult GetProduct()
        {
            try
            { 
                const string cacheKey = "product_list";
                if (!_cache.TryGetValue(cacheKey, out List<ProductForInitailloadingModel> productdata))
                {
                    var path= "/Upload/";
                productdata = datacontext.Data_tbl_Product.AsNoTracking().
                    Where(l => l.product_id != 0).
                                  Select(m => new ProductForInitailloadingModel
                                  {
                                      productid = m.product_id,
                                      name = m.product_name,
                                      image = path+ m.product_image,
                                      price = m.product_price,
                                  }).Take(12).ToList();
                    var cacheOptions = new MemoryCacheEntryOptions()
                           .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    _cache.Set(cacheKey, productdata, cacheOptions);
                }
                return Ok(productdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllProductItemGroupWise")]
        public IActionResult GetAllProductItemGroupWise(int itemgroupid)
        {
            try
            {
                var path = "/Upload/";
                var productdata = datacontext.Data_tbl_Product.AsNoTracking().
                    Where(l =>( l.product_id != 0) &&
                                l.product_item_group_id==itemgroupid).
                                  Select(m => new
                                  {
                                      productid = m.product_id,
                                      name = m.product_name,
                                      code = m.product_code,
                                      image = path + m.product_image,
                                      price = m.product_price,
                                      description = m.product_description,
                                  }).ToArray();
                return Ok(productdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProductById")]
        public IActionResult GetProductById(int productId,int userId)
        {
            try
            {
                var path = "/Upload/";

                var product = datacontext.Data_tbl_Product.AsNoTracking()
                    .Where(p => p.product_id == productId)
                    .Select(p => new
                    {
                        productid = p.product_id,
                        name = p.product_name,
                        price = p.product_price,
                        description = p.product_description,
                        code = p.product_code,
                        image = path + p.product_image,
                        iscartitem=p.Data_tbl_Cart.Any(c => c.cart_user_id == userId && c.cart_product_id == productId)

            }).FirstOrDefault();

                if (product == null)
                {
                    return NotFound(new{message = "Product not found"});
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new{message = ex.Message});
            }
        }

        [HttpGet("GetAllProduct")]
        public IActionResult GetAllProduct()
        {
            try
            {
                   var productdata = datacontext.Data_tbl_Product.AsNoTracking().
                        Where(l => l.product_id != 0).
                                      Select(m => new
                                      {
                                          productid = m.product_id,
                                          name = m.product_name,
                                          code=m.product_code,
                                          description=m.product_description,
                                          price = m.product_price,
                                      }).ToList();
                  
                return Ok(productdata);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
