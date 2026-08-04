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

        private const string ItemGroupListCacheKey = "itemGroups_list";
        private const string AllItemGroupCacheKey = "all_itemGroups_list";

        public ItemGroupController(IWebHostEnvironment environment, ApplicationDbContext context, IMemoryCache cache)
        {
            _environment = environment;
            datacontext = context;
            _cache = cache;
        }

        // Same helper pattern as ProductController - keep this identical across
        // controllers if you pull it into a shared base class later.
        private async Task<T> GetOrSetCacheAsync<T>(string key, Func<Task<T>> factory, int minutes = 10)
        {
            if (_cache.TryGetValue(key, out T cached))
                return cached;

            var data = await factory();
            var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes));
            _cache.Set(key, data, options);
            return data;
        }

        #region Item Group Save
        [HttpPost("ItemGroup")]
        public async Task<IActionResult> Create(ItemGroupModel data)
        {
            try
            {
                if (data.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(data.ImageFile.FileName);
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Upload");

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await data.ImageFile.CopyToAsync(stream);
                    }

                    data.item_group_image = fileName;
                }

                var saved = await SaveItemGroupAsync(data);
                return Ok(saved);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> SaveItemGroupAsync(ItemGroupModel data)
        {
            try
            {
                // Combined into one query instead of two separate .Any() round trips,
                // but kept as two checks so we can still report which field collided
                var duplicate = await datacontext.Data_tbl_Item_group.AsNoTracking()
                    .Where(m => m.item_group_id != data.item_group_id)
                    .Select(m => new { m.item_group_name, m.item_group_code })
                    .Where(m => m.item_group_name == data.item_group_name || m.item_group_code == data.item_group_code)
                    .FirstOrDefaultAsync();

                if (duplicate != null)
                {
                    if (duplicate.item_group_name == data.item_group_name)
                        return "Item group name already exist";
                    return "Item group code already exist";
                }

                if (data.item_group_id == 0)
                {
                    var productdata = new ItemGroupModel
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
                    var productdata = await datacontext.Data_tbl_Item_group.FirstOrDefaultAsync(m => m.item_group_id == data.item_group_id);
                    if (productdata != null)
                    {
                        productdata.item_group_name = data.item_group_name;
                        productdata.item_group_code = data.item_group_code;
                        productdata.item_group_image = data.item_group_image;
                        productdata.item_group_description = data.item_group_description;
                        productdata.item_group_user_id = data.item_group_user_id;
                        productdata.item_group_row_date = DateTime.UtcNow;
                    }
                }

                await datacontext.SaveChangesAsync();

                _cache.Remove(ItemGroupListCacheKey);
                _cache.Remove(AllItemGroupCacheKey);

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
        public async Task<IActionResult> GetItemGroupForProvider()
        {
            try
            {
                var itemgroup = await GetItemGroupsForProviderCachedAsync();
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        [HttpGet("GetAllItemGroup")]
        public async Task<IActionResult> GetAllItemGroup()
        {
            try
            {
                // Was completely uncached before - now cached like everything else
                var itemgroup = await GetOrSetCacheAsync(AllItemGroupCacheKey, async () =>
                {
                    return await datacontext.Data_tbl_Item_group.AsNoTracking()
                        .Where(l => l.item_group_id != 0)
                        .Select(m => new
                        {
                            item_group_id = m.item_group_id,
                            item_group_name = m.item_group_name,
                            item_group_code = m.item_group_code,
                            item_group_description = m.item_group_description,
                        })
                        .ToListAsync();
                });

                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetItemGroup")]
        public async Task<IActionResult> GetItemGroup(int itemgroupid)
        {
            try
            {
                var itemgroup = await datacontext.Data_tbl_Item_group.AsNoTracking()
                    .Where(l => l.item_group_id == itemgroupid)
                    .Select(m => new
                    {
                        item_group_id = m.item_group_id,
                        item_group_name = m.item_group_name,
                        item_group_code = m.item_group_code,
                        item_group_description = m.item_group_description,
                        item_group_image = m.item_group_image,
                    })
                    .FirstOrDefaultAsync();

                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllItemGroupWiseFiltering")]
        public async Task<IActionResult> GetAllItemGroupWiseFiltering()
        {
            try
            {
                var itemgroup = await GetItemGroupsForProviderCachedAsync();
                return Ok(itemgroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private Task<List<ItemGroupModelForProvider>> GetItemGroupsForProviderCachedAsync()
        {
            return GetOrSetCacheAsync(ItemGroupListCacheKey, async () =>
            {
                return await datacontext.Data_tbl_Item_group.AsNoTracking()
                    .Where(l => l.item_group_id != 0)
                    .Select(m => new ItemGroupModelForProvider
                    {
                        item_group_id = m.item_group_id,
                        item_group_name = m.item_group_name,
                    })
                    .ToListAsync();
            });
        }
    }
}