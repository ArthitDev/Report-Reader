using Microsoft.EntityFrameworkCore;

namespace Report_View.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add your DbSet properties here for your database tables
        // Example:
        // public DbSet<YourModel> YourModels { get; set; }
    }
} 