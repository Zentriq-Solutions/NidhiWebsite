
using NidhiWebsite.Models.Entity;
using Microsoft.EntityFrameworkCore;


namespace NidhiWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Data_tbl_User { get; set; }

    }
}
