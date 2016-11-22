using Microsoft.EntityFrameworkCore;

namespace PokemonService
{
    /*
    This configures the database for Entity Framework Core
    and is the entry point for databse operations.
    */
    public class PokemonDbContext : DbContext
    {
        public DbSet<Pokemon> Pokemon { get; set; }

        public PokemonDbContext() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=pokemon.db");
        }
    }
}