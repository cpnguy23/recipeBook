using Microsoft.EntityFrameworkCore;
using RecipeBook.Models;

namespace RecipeBook.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>
        options) : base(options)
        {
        } // the model file was created for the database, so we need to add it to the context file
        public DbSet<RecipeCustomers> Customers { get; set; }
        public DbSet<RecipePost> RecipePosts { get; set; }
    }
}
