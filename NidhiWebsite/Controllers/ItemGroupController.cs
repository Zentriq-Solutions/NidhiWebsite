using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        public ItemGroupController(IWebHostEnvironment environment, ApplicationDbContext context,IMemoryCache cache)
        {
            _environment = environment;
            datacontext = context;
            _cache = cache;
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
                if (allitemgroup.Any(m => m.item_group_name == data.item_group_name && (m.item_group_id != data.item_group_id)))
                {
                    return "Item group name already exist";
                }
                if (allitemgroup.Any(m => (m.item_group_code == data.item_group_code) && (m.item_group_id != data.item_group_id)))
                {
                    return "Item group code already exist";
                }

                var productdata = new ItemGroupModel();
                if (data.item_group_id == 0)
                {
                    productdata = new ItemGroupModel
                    {
                        item_group_name = data.item_group_name,
                        item_group_code = data.item_group_code,
                        item_group_image = data.item_group_image,
                        item_group_description = data.item_group_description,
                        item_group_user_id = data.item_group_user_id,
                        item_group_row_date = DateTime.UtcNow,
                    };

                    datacontext.Data_tbl_Item_group.Add(productdata);
                }
                else
                {
                    productdata = datacontext.Data_tbl_Item_group.FirstOrDefault(m => m.item_group_id == data.item_group_id);
                    if (productdata != null)
                    {
                        productdata.item_group_name = data.item_group_name;
                        productdata.item_group_code = data.item_group_code;
                        productdata.item_group_image = data.item_group_image;
                        productdata.item_group_description = data.item_group_description;
                        productdata.item_group_user_id = data.item_group_user_id;
                        productdata.item_group_row_date = DateTime.UtcNow;
                        datacontext.Data_tbl_Item_group.Update(productdata);
                    }
                }
                datacontext.SaveChanges();
                _cache.Remove("itemGroups_list");
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
                const string cacheKey = "itemGroups_list";

                if (!_cache.TryGetValue(cacheKey, out List<ItemGroupModelForProvider> itemgroup))
                {
                    itemgroup = datacontext.Data_tbl_Item_group.AsNoTracking().
                                Where(l => l.item_group_id != 0).
                                Select(m => new ItemGroupModelForProvider
                                {
                                    item_group_id = m.item_group_id,
                                    item_group_name = m.item_group_name,
                                }).ToList();
                    var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, itemgroup, cacheOptions);
                }
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        [HttpGet("GetAllItemGroup")]
        public IActionResult GetAllItemGroup()
        {
            try
            {
                var itemgroup = datacontext.Data_tbl_Item_group.AsNoTracking().
                                Where(l => l.item_group_id != 0).
                                Select(m => new
                                {
                                    item_group_id = m.item_group_id,
                                    item_group_name = m.item_group_name,
                                    item_group_code = m.item_group_code,
                                    item_group_description = m.item_group_description,
                                }).ToArray();
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetItemGroup")]
        public IActionResult GetItemGroup(int itemgroupid)
        {
            try
            {
                var itemgroup = datacontext.Data_tbl_Item_group.AsNoTracking().
                                Where(l => l.item_group_id == itemgroupid).
                                Select(m => new
                                {
                                    item_group_id = m.item_group_id,
                                    item_group_name = m.item_group_name,
                                    item_group_code = m.item_group_code,
                                    item_group_description = m.item_group_description,
                                    item_group_image = m.item_group_image,
                                }).FirstOrDefault();
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllItemGroupWiseFiltering")]
        public IActionResult GetAllItemGroupWiseFiltering()
        {
            try
            {
                const string cacheKey = "itemGroups_list";

                if (!_cache.TryGetValue(cacheKey, out List<ItemGroupModelForProvider> itemgroup))
                {
                    itemgroup = datacontext.Data_tbl_Item_group.AsNoTracking().
                                Where(l => l.item_group_id != 0).
                                Select(m => new ItemGroupModelForProvider
                                {
                                    item_group_id = m.item_group_id,
                                    item_group_name = m.item_group_name,
                                }).ToList();
                    var cacheOptions = new MemoryCacheEntryOptions()
           .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                    _cache.Set(cacheKey, itemgroup, cacheOptions);
                }
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
