using Microsoft.EntityFrameworkCore;
using MovieAPI2.Entities;

namespace MovieAPI2.Data
{
    public class MovieDBContext : DbContext
    {
        public MovieDBContext(DbContextOptions<MovieDBContext> options) : base(options) {

        }

        public DbSet<Movie> Movie { get; set; }

        public DbSet<Person> Person { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
