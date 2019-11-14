using Microsoft.EntityFrameworkCore;
 
namespace Videogames.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<Game> Games {get;set;}

        public DbSet<Community> Communities {get;set;}
    }
}