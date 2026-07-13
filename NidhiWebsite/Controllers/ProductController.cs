using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NidhiWebsite.Data;
using NidhiWebsite.Models.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NidhiWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext datacontext;
        public ProductController(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            datacontext = context;
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
                };
                datacontext.Data_tbl_Product.Add(productdata);
                datacontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
