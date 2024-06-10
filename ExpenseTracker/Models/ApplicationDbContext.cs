using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models
{
    public class ApplicationDbContext:DbContext
    {
        //the application db context instance will be created through dependency injection  so  update program.cs file
        public ApplicationDbContext(DbContextOptions options):base(options)
        {
            
        }
        //through this constructor we have pass our dbprovider whether it is mysql or sql server and corresponding db connection string
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
