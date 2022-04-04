using Microsoft.EntityFrameworkCore;

namespace CodingExercise2.Server.Models
{
    /// <summary>
    /// The DatabaseContext class is a subclass of DbContext that's aware of the Comments table. It's used by the DatabaseRepository class.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}