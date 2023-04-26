using ASPNETMVCPHONEBOOK.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ASPNETMVCPHONEBOOK.Data
{
    public class MVCDemoDbContext : DbContext
    {
        public MVCDemoDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<Contact> Contacts { get; set; }
    }
}
