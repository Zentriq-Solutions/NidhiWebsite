using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NidhiWebsite.Data;
using NidhiWebsite.Models.Entity;

namespace NidhiWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemGroupController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext datacontext;
        public ItemGroupController(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            datacontext = context;
        }

        #region Item Group Save
        [HttpPost("ItemGroup")]
        public async Task<IActionResult> Create(ItemGroupModel data)
        {
            try
            {
                if (data.ImageFile != null)
                {
                    // Create a unique filename
                    string fileName = Guid.NewGuid().ToString() +
                                      Path.GetExtension(data.ImageFile.FileName);

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
                        await data.ImageFile.CopyToAsync(stream);
                    }

                    // Save filename in database
                    data.item_group_image = fileName;
                }
                var saved = SaveItemGroup(data);
                    return Ok(saved);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string SaveItemGroup([FromForm] ItemGroupModel data)
        {
            try
            {
                var allitemgroup = datacontext.Data_tbl_Item_group.AsNoTracking().
                                   Select(m => new
                                   {
                                       item_group_id = m.item_group_id,
                                       item_group_name = m.item_group_name,
                                       item_group_code = m.item_group_code,
                                   });
                if(allitemgroup.Any(m => m.item_group_name == data.item_group_name && (m.item_group_id !=data.item_group_id)))
                {
                    return "Item group name already exist";
                }
                if(allitemgroup.Any(m => (m.item_group_code == data.item_group_code) && (m.item_group_id != data.item_group_id)))
                {
                    return "Item group code already exist";
                }

                var productdata = new ItemGroupModel();
                productdata = new ItemGroupModel
                {
                    item_group_name= data.item_group_name,
                    item_group_code = data.item_group_code,
                    item_group_image = data.item_group_image,
                    item_group_description = data.item_group_description,
                    item_group_user_id = data.item_group_user_id,
                    item_group_row_date = DateTime.UtcNow,
                };
                datacontext.Data_tbl_Item_group.Add(productdata);
                datacontext.SaveChanges();
                return "success";
            }
            catch (Exception)
            {
                return "failed";
            }
        }
        #endregion
        #region Item Group For Provider
        [HttpGet("ItemGroupForProvider")]
        public IActionResult GetItemGroupForProvider()
        {
            try
            {
                var itemgroup = datacontext.Data_tbl_Item_group.AsNoTracking().
                                Where(l =>l.item_group_id!=0).
                                Select(m => new
                                {
                                    item_group_id = m.item_group_id,
                                    item_group_name = m.item_group_name,
                                }).ToArray();
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

    }
}
