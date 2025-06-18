using Microsoft.EntityFrameworkCore;
namespace SDAProject.Models
{
    public class KeyLoggerDbContext : DbContext
    {
        public KeyLoggerDbContext(DbContextOptions<KeyLoggerDbContext> options) : base(options)
        {
        }
        DbSet<UsersModel> Users { get; set; }

    }
}
