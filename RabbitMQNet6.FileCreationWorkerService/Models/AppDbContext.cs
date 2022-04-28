using Microsoft.EntityFrameworkCore;

namespace RabbitMQNet6.FileCreationWorkerService.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
